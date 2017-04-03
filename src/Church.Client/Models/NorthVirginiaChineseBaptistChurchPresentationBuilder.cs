// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NorthVirginiaChineseBaptistChurchPresentationBuilder.cs" company="Church">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Microsoft.Office.Core;
    using Microsoft.Office.Interop.PowerPoint;

    public class NorthVirginiaChineseBaptistChurchPresentationBuilder : PowerPointPresentationBuilder
    {
        private DataTransfer _transfer;
        public Bulletin Bulletin { get; internal set; }
        private string _path;
        public CultureInfo CurrentCulture { get; internal set; }

        public NorthVirginiaChineseBaptistChurchPresentationBuilder(DataTransfer transfer, Bulletin bulletin, string path)
            : base("SimSun", "SimSun")
        {
            this._transfer = transfer;
            this.Bulletin = bulletin;
            this._path = path;
            this.CurrentCulture = CultureInfo.CreateSpecificCulture("zh-CN");
        }

        protected void MakeTitleSlides()
        {
            this.CreateTitleSlide("主日敬拜", "北维州华人浸信会" + Environment.NewLine + this.Bulletin.Date?.ToString("yyyy年M月d日"));
            this.CreateContentSlideWithTitleOnly(200, "请保持肃静!", string.Empty, "也请把手机铃声关掉或设为震动!");
        }

        private IEnumerable<LyricsSection> GetLyrics(string name, string source)
        {
            var hymn = this._transfer.GetHymn(name, source);
            Trace.Assert(null != hymn);
            return Utilities.ParseFromJson<IEnumerable<LyricsSection>>(hymn.Lyrics);
        }

        [SectionPatterns("赞美诗.*")]
        protected void MakePraiseSlides(params string[] items)
        {
            Trace.Assert(2 <= items.Length);
            var lyrics = this.GetLyrics(items[1], string.Empty);
            this.MakeHymnSlide("唱诗：" + items[1], lyrics, HymnSlideType.Contemporary);
        }

        [SectionPatterns(".*序乐")]
        protected void MakePreludeSlides(params string[] items)
        {
            Trace.Assert(2 <= items.Length);
            var parts = items[1].Split('、');
            Trace.Assert(2 <= parts.Length);
            var lyrics = this.GetLyrics(parts[1], "颂主新歌#" + parts[0]);
            var slides = this.MakeHymnSlide(parts[1], lyrics, HymnSlideType.Contemporary);
            foreach (var s in slides)
            {
                this.AttachBackground(s, Path.Combine(this._path, "Backgroud1.jpg"), (float)1.24, (float)0.95);
            }
        }

        [SectionPatterns(".*唱诗")]
        protected void MakeHymnSlides(params string[] items)
        {
            Trace.Assert(2 <= items.Length);
            var names = items[1].Split('–', '-');
            if (names.Length == 1)
            {
                var lyrics = this.GetLyrics(names[0], string.Empty);
                this.MakeHymnSlide(string.Format("唱诗: {0}", names[0]), lyrics, HymnSlideType.Contemporary);
            }
            else
            {
                var lyrics = this.GetLyrics(names[0], "颂主新歌#" + names[1]);
                this.MakeHymnSlide(string.Format("唱诗: {0} – {1}", names[1], names[0]), lyrics, HymnSlideType.Traditional);
            }
        }

        [SectionPatterns("三一颂")]
        protected void MakeDoxologySlides(params string[] items)
        {
            Trace.Assert(2 <= items.Length);
            var lyrics = this.GetLyrics("三一颂", string.Empty);
            var slides = this.MakeHymnSlide("三一颂", lyrics, HymnSlideType.Contemporary);
            foreach (var s in slides)
            {
                this.AttachBackground(s, Path.Combine(this._path, "Backgroud1.jpg"), (float)1.24, (float)0.95);
            }
        }

        [SectionPatterns("迎新")]
        protected void MakeWelcomeSlides(params string[] items)
        {
            var lyrics = this.GetLyrics("欢迎歌", string.Empty);
            var slides = this.MakeHymnSlide("欢迎歌", lyrics, HymnSlideType.Contemporary);
        }

        private IEnumerable<Slide> MakeHymnSlide(string title, IEnumerable<LyricsSection> lyrics, HymnSlideType type)
        {
            var lstSlide = new List<Slide>();

            if (0 == lyrics.Count())
            {
                var slide = this.CreateContentSlideWithTitleContent(title, 40, "Could not get the lyrics.", 36, true);
                lstSlide.Add(slide);
                return lstSlide;
            }

            var refrain = lyrics.FirstOrDefault(s => LyricsSectionType.Refrain == s.Type);
            var refrainText = null == refrain ? null : string.Join(Environment.NewLine, refrain.Text);
            int loop = 0;
            foreach (var m in lyrics)
            {
                if (LyricsSectionType.Refrain == m.Type)
                {
                    continue;
                }
                var text = string.Join(Environment.NewLine, m.Text);
                var line = (HymnSlideType.Traditional == type) ? (++loop).ToString() + ". " + text : text;
                var slide = this.CreateContentSlideWithTitleContent(title, 40, line, 36, true);
                if (null != refrainText && LyricsSectionType.Main == m.Type)
                {
                    var textRange = slide.Shapes[2].TextFrame.TextRange.InsertAfter(Environment.NewLine + Environment.NewLine);
                    textRange = slide.Shapes[2].TextFrame.TextRange.InsertAfter(refrainText);
                    textRange.Font.Underline = MsoTriState.msoTrue;
                }
                lstSlide.Add(slide);
            }

            int total = lstSlide.Count;
            if (1 < total)
            {
                loop = 0;
                foreach (var slide in lstSlide)
                {
                    slide.Shapes[1].TextFrame.TextRange.Text = title + string.Format("({0}/{1})", ++loop, total);
                }
            }

            return lstSlide;
        }

        [SectionPatterns(".*祷告")]
        protected void MakePrayerSlide(params string[] items) 
        {
            var slide = this.CreateContentSlideWithTitleOnly(450, "祷告");
            this.AttachBackground(slide, Path.Combine(this._path, "Backgroud1.jpg"), (float)1.24, (float)0.95);
        }

        [SectionPatterns("读经")]
        protected void MakeScriptureSlide(params string[] items)
        {
            Trace.Assert(2 <= items.Length);
            var abbreviation = items[1];
            var scripture = this._transfer.GetBibleVerses(abbreviation);
            var verses = scripture.Verses.Select(v => string.Format("{0} {1}", v.Order, v.Text));

            this.CreateContentSlideWithTitleContent(abbreviation, 36, verses, 32);
        }

        [SectionPatterns("见证")]
        protected void MakeTestimony(params string[] items)
        {
            this.CreateContentSlideWithTitleOnly(200, items[1]);
        }

        [SectionPatterns("证道")]
        protected void MakeSermonSlides(params string[] items)
        {
            Trace.Assert(2 <= items.Length);
            var title = items[1];
            var abbreviation = (this.Bulletin as NorthVirginiaChineseBaptistChurchBulletin).MessageVerseAbbr;
            this.CreateContentSlideWithTitleOnly(220, title, string.Empty, abbreviation);
            var lstLine = new List<string>();
            Trace.Assert(this.Bulletin is NorthVirginiaChineseBaptistChurchBulletin);
            foreach (var l in (this.Bulletin as NorthVirginiaChineseBaptistChurchBulletin).SermonCompendium)
            {
                if ("前言".Equals(l.Trim()))
                {
                    lstLine.Clear();
                }
                else if ("讨论题".Equals(l.Trim()))
                {
                    break;
                }
                lstLine.Add(l);
            }
            this.CreateContentSlideWithTitleContent(title, 40, lstLine, 32);
        }

        [SectionPatterns("圣餐")]
        protected void MakeCommunionSlide(params string[] items)
        {
            //林前11：23-26
            string[] lines =
            {
                "11:23 我当日传给你们的，原是从主领受的，就是主耶稣被卖的那一夜，拿起饼来，",
                "11:24 祝谢了，就擘开，说：「这是我的身体，为你们舍的，你们应当如此行，为的是记念我。」",
                "11:25 饭後，也照样拿起杯来，说：「这杯是用我的血所立的新约，你们每逢喝的时候，要如此行，为的是记念我。」",
                "11:26 你们每逢吃这饼，喝这杯，是表明主的死，直等到他来。"
            };

            this.CreateContentSlideWithTitleContent("圣餐 (Communion)-歌林多前书 (1 Corinthians)", 40, lines, 32);
        }

        [SectionPatterns("报告")]
        protected void MakeAnnouncementsSlide(params string[] items)
        {
            this.CreateContentSlideWithTitleOnly(180, "报告");
        }

        [SectionPatterns(".*祝福", "殿乐", "准新娘赠礼会")]
        protected void SkipSection(params string[] items)
        {
        }

        public override void Make()
        {
            var pptApplication = new Application
            {
                Visible = MsoTriState.msoTrue
            };
            pptApplication.Activate();
            this._presentation = pptApplication.Presentations.Add();
            this._presentation.ApplyTemplate(Path.Combine(this._path, "ServiceTemplate.potx"));

            this.MakeTitleSlides();
            Trace.Assert(this.Bulletin is NorthVirginiaChineseBaptistChurchBulletin);
            foreach (var item in (this.Bulletin as NorthVirginiaChineseBaptistChurchBulletin).WorshipProgram)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    continue;
                }
                this.MakeSlides(s => new Regex(s).IsMatch(item.Name), () => new[] { item.Name, item.Value, item.NameInEnglish });
            }

            string filename = Path.Combine(this._path, string.Format("{0}_service.pptx", this.Bulletin.Date?.ToString("yyyyMMdd")));
            this._presentation.SaveAs(filename, Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsDefault, MsoTriState.msoTrue);
            //this._presentation.Close();
            //pptApplication.Quit();
        }
    }
}