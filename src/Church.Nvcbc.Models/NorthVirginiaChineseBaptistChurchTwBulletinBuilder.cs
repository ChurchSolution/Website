namespace Church.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    public class NorthVirginiaChineseBaptistChurchTwBulletinBuilder : BulletinTextBuilder
    {
        private readonly char[] Commas = new[] { ',', '，' };
        private readonly char[] Semicollons = new[] { ';', '；' };
        private readonly char[] Collons = new[] { ':', '：' };

        const string WordFromPasterSectionKey = "牧者的話  WORD FROM  PASTOR";
        const string LastWeekDataSectionKey = "上周主日出席人數：";
        const string NextWeekServicesSectionKey = "下周主日崇拜事奉分工";
        const string HeaderSectionKey = "北  維  州  華  人  浸  信  會";
        const string WorshipProgramSectionKey = "主 日 崇 拜";
        const string ServicesSectionKey = "主日崇拜事奉分工";
        const string MemorizedVersesSectionKey = "記憶經文  MEMORY VERSE OF THE WEEK";
        const string AnnouncementsSectionKey = "報告事項   ANNOUNCEMENTS";
        const string PrayerRequestsSectionKey = "代禱事項  PRAYER REQUESTS";
        const string GroupLeadersSectionKey = "事工組負責人  GROUP LEADERS";
        const string ActivitiesSectionKey = "教會活動  ACTIVITIES";
        const string FamilyWorshipSectionKey = "本周家庭崇拜";
        const string SermonCompendiumSectionKey = "證道大綱：";
        const string SermonCompendiumSectionKey2 = "見證分享：";
        const string WeeklyReadingSectionKey = "本周讀經表";
        const string ChurchOfficeSectionKey = "* 教會辦公室地址";

        public NorthVirginiaChineseBaptistChurchTwBulletinBuilder() : base(CultureInfo.CreateSpecificCulture("zh-TW")) { }

        [SectionSeparator(WordFromPasterSectionKey)]
        protected IEnumerable<string> ProcessWordFromPastor(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(lines, "lines", "No input line.");

            var cells = lines.First().Split('-');
            ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == cells.Length, "Could not find the Pastor name.");
            bulletin.PastorName = cells[1].Trim();

            var wordFromPastor = new List<string>();
            wordFromPastor.AddRange(lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l)));

            bulletin.WordFromPastor = wordFromPastor;
            return Enumerable.Empty<string>();
        }

        [SectionSeparator(LastWeekDataSectionKey)]
        protected IEnumerable<string> ProcessLastWeekData(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(lines, "lines", "No input line.");
            ExceptionUtilities.ThrowFormatExceptionIfFalse(2 <= lines.Count(), "At least two lines are expected.");

            var lastWeekData = new Dictionary<string, decimal>();
            this.ProcessColonTable(new List<string>(new string[] { lines.First() }), items =>
            {
                switch (items[0].Trim())
                {
                    case "成人":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '主日成人'.");
                        lastWeekData.Add("主日成人", ParseNumber(items[1].Trim()));
                        break;
                    case "兒童":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '主日儿童'.");
                        lastWeekData.Add("主日兒童", ParseNumber(items[1].Trim()));
                        break;
                }
            });
            this.ProcessColonTable(new List<string>(new string[] { lines.Skip(1).First() }), items =>
            {
                switch (items[0].Trim())
                {
                    case "成人":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '周间成人'.");
                        lastWeekData.Add("周間成人", ParseNumber(items[1].Trim()));
                        break;
                    case "兒童":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '周间儿童'.");
                        lastWeekData.Add("周間兒童", ParseNumber(items[1].Trim()));
                        break;
                }
            });
            this.ProcessColonTable(lines.Skip(2), items =>
            {
                switch (items[0].Trim())
                {
                    case "上周總奉獻":
                    case "上兩周總奉獻":
                    case "前周總奉獻":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '奉獻總計'.");
                        lastWeekData.Add("奉獻總計", ParseMoney(items[1].Trim()));
                        break;
                    case "一般":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '一般奉獻'.");
                        lastWeekData.Add("一般奉獻", ParseMoney(items[1].Trim()));
                        break;
                    case "建堂":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '建堂奉獻'.");
                        lastWeekData.Add("建堂奉獻", ParseMoney(items[1].Trim()));
                        break;
                    case "愛心":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '愛心奉獻'.");
                        lastWeekData.Add("愛心奉獻", ParseMoney(items[1].Trim()));
                        break;
                    case "彼岸":
                    case "彼岸基金":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '彼岸基金'.");
                        lastWeekData.Add("彼岸基金", ParseMoney(items[1].Trim()));
                        break;
                }
            });

            bulletin.LastWeekData = lastWeekData;
            return Enumerable.Empty<string>();
        }

        [SectionSeparator(NextWeekServicesSectionKey)]
        protected IEnumerable<string> ProcessServicesOfNextWeek(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(lines, "lines", "No input line.");
            ExceptionUtilities.ThrowFormatExceptionIfFalse(1 <= lines.Count(), "At least one line is expected.");

            var services = bulletin.Services ?? new Dictionary<string, NorthVirginiaChineseBaptistChurchBulletin.ServiceItem>();
            IEnumerable<string> res = this.ProcessWordTable(lines.Skip(1), s => s, 2, cells =>
            {
                var name = cells[0].Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    return;
                }

                ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == cells.Length, "Could not find the value for the key '{0}'.", name);
                NorthVirginiaChineseBaptistChurchBulletin.ServiceItem item;
                var people = cells[1].Split(Commas);
                if (!services.TryGetValue(name, out item))
                {
                    item = NorthVirginiaChineseBaptistChurchBulletin.ServiceItem.Create(name);
                    services.Add(name, item);
                }
                item.PeopleForNext = people;
            });

            bulletin.Services = services;
            return res;
        }

        [SectionSeparator(HeaderSectionKey)]
        protected IEnumerable<string> ProcessHeader(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(lines, "lines", "No input line.");

            var services = bulletin.Services ?? new Dictionary<string, NorthVirginiaChineseBaptistChurchBulletin.ServiceItem>();
            this.ProcessColonTable(lines, items =>
            {
                var name = items[0].Trim();
                switch (name)
                {
                    case "講員":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the speaker.");
                        bulletin.Speaker = items[1].Trim();
                        break;
                    case "司琴":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the pianist.");
                        NorthVirginiaChineseBaptistChurchBulletin.ServiceItem item;
                        if (!services.TryGetValue(name, out item))
                        {
                            item = NorthVirginiaChineseBaptistChurchBulletin.ServiceItem.Create(name);
                            services.Add(name, item);
                        }
                        item.People = new[] { items[1].Trim() };
                        break;
                }
            });

            bulletin.Services = services;
            return Enumerable.Empty<string>();
        }

        [SectionSeparator(WorshipProgramSectionKey)]
        protected IEnumerable<string> ProcessWorshipProgram(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(lines, "lines", "No input line.");
            ExceptionUtilities.ThrowFormatExceptionIfFalse(3 <= lines.Count(), "At least three lines are expected.");

            var cells = lines.Skip(1).First().Split(' ');
            DateTime date;
            ExceptionUtilities.ThrowFormatExceptionIfFalse(DateTime.TryParse(cells[0], out date), "Could not parse the date '{0}'", cells[0]);
            bulletin.Date = date;
            bulletin.DateString = date.ToString("D", this.Culture.DateTimeFormat);

            var worshipProgram = new List<NorthVirginiaChineseBaptistChurchBulletin.ProgramItem>();
            foreach (string l in lines.Skip(2).Take(lines.Count() - 3).Where(l => !string.IsNullOrWhiteSpace(l)))
            {
                cells = l.Split('\t');
                ExceptionUtilities.ThrowFormatExceptionIfFalse(3 == cells.Length, "Three columns are expected in '{0}'.", l);
                Trace.Assert(3 == cells.Length, WorshipProgramSectionKey);
                var item = NorthVirginiaChineseBaptistChurchBulletin.ProgramItem.Create(
                    cells[0].TrimStart('☆').Trim(), cells[1].Trim(), cells[2].Trim());
                worshipProgram.Add(item);
                if ("Sermon".Equals(item.NameInEnglish, StringComparison.OrdinalIgnoreCase)
                    || "Testimony".Equals(item.NameInEnglish, StringComparison.OrdinalIgnoreCase))
                {
                    bulletin.MessageTitle = item.Value;
                }
                else if ("Scripture".Equals(item.NameInEnglish, StringComparison.OrdinalIgnoreCase))
                {
                    bulletin.MessageVerseAbbr = item.Value;
                }
            }

            bulletin.WorshipProgram = worshipProgram;
            return Enumerable.Empty<string>();
        }

        [SectionSeparator(ServicesSectionKey)]
        protected IEnumerable<string> ProcessServices(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(lines, "lines", "No input line.");
            ExceptionUtilities.ThrowFormatExceptionIfFalse(1 <= lines.Count(), "At least one line is expected.");

            var services = bulletin.Services ?? new Dictionary<string, NorthVirginiaChineseBaptistChurchBulletin.ServiceItem>();
            IEnumerable<string> res = this.ProcessWordTable(lines.Skip(1), s => s, 2, cells =>
            {
                var name = cells[0].Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    return;
                }

                ExceptionUtilities.ThrowFormatExceptionIfFalse(2 <= cells.Length, "Could not find the value for '{0}'.", name);
                NorthVirginiaChineseBaptistChurchBulletin.ServiceItem item;
                if (!services.TryGetValue(name, out item))
                {
                    item = NorthVirginiaChineseBaptistChurchBulletin.ServiceItem.Create(name);
                    services.Add(name, item);
                }
                item.People = cells[1].Trim().Split(Commas);
            });

            bulletin.Services = services;
            return res;
        }

        [SectionSeparator(MemorizedVersesSectionKey)]
        protected IEnumerable<string> ProcessMemorizedVerses(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(lines, "lines", "No input line.");
            ExceptionUtilities.ThrowFormatExceptionIfFalse(1 <= lines.Count(), "At least one line is expected.");

            var memorizedVerses = new List<string>();
            this.ProcessTabTable(lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l)), items =>
            {
                ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the verse.");
                memorizedVerses.Add(items[1].Trim());
            });

            bulletin.MemorizedVerses = memorizedVerses;
            return Enumerable.Empty<string>();
        }

        [SectionSeparator(AnnouncementsSectionKey)]
        protected IEnumerable<string> ProcessAnnouncements(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(lines, "lines", "No input line.");
            ExceptionUtilities.ThrowFormatExceptionIfFalse(1 <= lines.Count(), "At least one line is expected.");

            bulletin.Announcements = lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            return Enumerable.Empty<string>();
        }

        [SectionSeparator(PrayerRequestsSectionKey)]
        protected IEnumerable<string> ProcessPrayerRequests(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(lines, "lines", "No input line.");
            ExceptionUtilities.ThrowFormatExceptionIfFalse(1 <= lines.Count(), "At least one line is expected.");

            bulletin.PrayerRequests = lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            return Enumerable.Empty<string>();
        }

        [SectionSeparator(GroupLeadersSectionKey)]
        protected IEnumerable<string> ProcessGroupLeaders(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(lines, "lines", "No input line.");
            ExceptionUtilities.ThrowFormatExceptionIfFalse(1 <= lines.Count(), "At least one line is expected.");

            // TODO: Do nothing?
            return Enumerable.Empty<string>();
        }

        [SectionSeparator(ActivitiesSectionKey)]
        protected IEnumerable<string> ProcessActivities(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(lines, "lines", "No input line.");
            ExceptionUtilities.ThrowFormatExceptionIfFalse(1 <= lines.Count(), "At least one line is expected.");

            var activities = new List<NorthVirginiaChineseBaptistChurchBulletin.ActivityItem>();
            this.ProcessTabTable(lines.Skip(1).Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("*")), cells =>
            {
                var name = cells[0].Trim();
                var time = cells[1].Trim();
                var location = cells[2].Trim();
                var phone = cells[3].Trim();
                var map = string.Empty;
                if ("崇拜聚會點".Equals(location, StringComparison.OrdinalIgnoreCase))
                {
                    map = "<a target=\"_blank\" href=\"http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=5004+Sideburn+Rd,Fairfax,+VA+22032&amp;aq=&amp;sll=37.0625,-95.677068&amp;sspn=41.411029,77.431641&amp;ie=UTF8&amp;hq=&amp;hnear=5004+Sideburn+Rd,+Fairfax,+Virginia+22032&amp;t=m&amp;z=14&amp;iwloc=r0&amp;ll=38.818833,-77.309889\" style=\"color:#0000FF;text-align:left\">地圖</a>";
                }
                else if ("教會辦公室".Equals(location, StringComparison.OrdinalIgnoreCase))
                {
                    map = "<a target=\"_blank\" href=\"http://maps.google.com/maps?f=q&amp;source=embed&amp;hl=en&amp;geocode=&amp;q=11094+Lee+Highway,+A103,+Fairfax,+VA+22030&amp;aq=&amp;sll=38.818833,-77.309889&amp;sspn=0.033437,0.064459&amp;ie=UTF8&amp;hq=&amp;hnear=11094+Lee+Hwy,+Fairfax,+Virginia+22030&amp;t=m&amp;z=14&amp;ll=38.853398,-77.328402\" style=\"color:#0000FF;text-align:left\">地圖</a>";
                }
                activities.Add(NorthVirginiaChineseBaptistChurchBulletin.ActivityItem.Create(name, time, location, map, phone));
            });

            bulletin.Activities = activities;
            return Enumerable.Empty<string>();
        }

        [SectionSeparator(FamilyWorshipSectionKey)]
        protected IEnumerable<string> ProcessFamilyWorship(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(lines, "lines", "No input line.");
            ExceptionUtilities.ThrowFormatExceptionIfFalse(2 <= lines.Count(), "At least two lines are expected.");

            // Example
            //詩歌: 《主，我敬拜你》，經文：可10：6-9
            //網上播放:	 http://www.youtube.com/watch?v=WsmnClrUBrY&feature=related
            var firstLine = lines.Skip(1).First();
            var cells = firstLine.Split(new string[] { "》，經文：" }, StringSplitOptions.None);
            ExceptionUtilities.ThrowFormatExceptionIfFalse(2 <= cells.Length, "Could not find the verse in '{0}'.", firstLine);
            var items = cells[0].Split('《');
            ExceptionUtilities.ThrowFormatExceptionIfFalse(2 <= items.Length, "Could not find the hymn in '{0}'.", firstLine);
            var praiseName = items[1].Trim();
            var verse = cells[1].Trim();
            var secondLine = lines.Skip(2).First();
            var parts = secondLine.Split('\t');
            ExceptionUtilities.ThrowFormatExceptionIfFalse(2 <= parts.Length, "Could not find the URL in '{0}'.", secondLine);
            var praiseUri = parts[1].Trim();

            bulletin.FamilyWorship = NorthVirginiaChineseBaptistChurchBulletin.FamilyWorshipItem.Create(praiseName, praiseUri, verse);
            return Enumerable.Empty<string>();
        }

        [SectionSeparator(WeeklyReadingSectionKey)]
        protected IEnumerable<string> ProcessWeeklyReading(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            return Enumerable.Empty<string>();
        }

        [SectionSeparator(SermonCompendiumSectionKey, SermonCompendiumSectionKey2)]
        protected IEnumerable<string> ProcessSermonCompendium(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(lines, "lines", "No input line.");

            bulletin.SermonCompendium = lines.Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l) && !"1".Equals(l, StringComparison.OrdinalIgnoreCase) && !"\t".Equals(l, StringComparison.OrdinalIgnoreCase))
                .ToArray();
            return Enumerable.Empty<string>();
        }

        [SectionSeparator(ChurchOfficeSectionKey)]
        protected IEnumerable<string> ProcessChurchOffice(NorthVirginiaChineseBaptistChurchBulletin bulletin, IEnumerable<string> lines)
        {
            return Enumerable.Empty<string>();
        }

        private void ProcessTabTable(IEnumerable<string> lines, Action<string[]> addCell)
        {
            Trace.Assert(null != lines, "The params 'lines' cannot be null.");
            foreach (string l in lines)
            {
                addCell(l.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        private void ProcessColonTable(IEnumerable<string> lines, Action<string[]> addItem)
        {
            Trace.Assert(null != lines, "The params 'lines' cannot be null.");
            this.ProcessTabTable(lines, cells =>
            {
                foreach (string c in cells)
                {
                    addItem(c.Split('：'));
                }
            });
        }
    }
}
