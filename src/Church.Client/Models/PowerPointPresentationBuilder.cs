// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PowerPointPresentationBuilder.cs" company="Church">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Office.Core;
    using Microsoft.Office.Interop.PowerPoint;

    internal enum HymnSlideType
    {
        Traditional,
        Contemporary
    }

    public abstract class PowerPointPresentationBuilder : AbstractPresentationBuilder
    {
        //this.CreateSlide(PpSlideLayout.ppLayoutTitle); // Title slide = 1
        //this.CreateSlide(PpSlideLayout.ppLayoutText); // Title and Content = 2
        //this.CreateSlide(PpSlideLayout.ppLayoutTwoColumnText); // Section Header = 3
        //this.CreateSlide(PpSlideLayout.ppLayoutTable); // Two Content = 4
        //this.CreateSlide(PpSlideLayout.ppLayoutTextAndChart); // Comparison = 5
        //this.CreateSlide(PpSlideLayout.ppLayoutChartAndText); // Title Only = 6
        //this.CreateSlide(PpSlideLayout.ppLayoutOrgchart); // Blank = 7
        //this.CreateSlide(PpSlideLayout.ppLayoutChart); // Content with Caption = 8
        //this.CreateSlide(PpSlideLayout.ppLayoutTextAndClipart); // Picture with Caption = 9
        //this.CreateSlide(PpSlideLayout.ppLayoutClipartAndText); // Title with Vertical Text = 10
        //this.CreateSlide(PpSlideLayout.ppLayoutTitleOnly); // Vertical Title and Text = 11
        //this.CreateSlide(PpSlideLayout.ppLayoutBlank); // Title, Text, and Content = 12

        protected Presentation _presentation;
        protected string _nameAscii = "SimSun";
        protected string _nameFarEast = "SimSun";

        public PowerPointPresentationBuilder(string nameAscii, string nameFarEast)
        {
            this._nameAscii = nameAscii;
            this._nameFarEast = nameFarEast;
        }

        protected void MakeSlides(Func<string, bool> predicate, Func<object[]> getParameters)
        {
            var members = this.GetType().GetMembers(BindingFlags.NonPublic | BindingFlags.Instance);
            Trace.Assert(null != members);
            var member = members.FirstOrDefault(
                m => m.GetCustomAttributes(typeof(SectionPatternsAttribute), false)
                    .SelectMany(a => (a as SectionPatternsAttribute).Patterns)
                    .Any(predicate));
            Trace.Assert(null != member);
            this.GetType().InvokeMember(member.Name,
                BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null, this, getParameters());
        }

        protected Slide CreateSlide(PpSlideLayout layout)
        {
            Debug.Assert(null != this._presentation);
            var customLayout = this._presentation.SlideMaster.CustomLayouts[layout];
            var slides = this._presentation.Slides;
            return slides.AddSlide(slides.Count + 1, customLayout);
        }

        protected Slide CreateTitleSlide(string title, string subtitle)
        {
            var slide = this.CreateSlide(PpSlideLayout.ppLayoutTitle);
            this.SetTextRange(slide.Shapes[1].TextFrame.TextRange, title, 80, MsoTriState.msoTrue);
            this.SetTextRange(slide.Shapes[2].TextFrame.TextRange, subtitle, 32, MsoTriState.msoFalse);

            return slide;
        }

        protected Slide CreateContentSlideWithTitleOnly(float top, params string[] lines)
        {
            var slide = this.CreateSlide(PpSlideLayout.ppLayoutChartAndText);
            var content = string.Join(Environment.NewLine, lines);
            this.SetTextRange(slide.Shapes[1].TextFrame.TextRange, content, 48, MsoTriState.msoTrue);
            slide.Shapes[1].Top = top;

            return slide;
        }

        protected Slide CreateContentSlideWithTitleContent(string title, float titleSize, string content, float contentSize, bool isCenter)
        {
            var slide = this.CreateSlide(PpSlideLayout.ppLayoutText);

            this.SetTextRange(slide.Shapes[1].TextFrame.TextRange, title, titleSize, MsoTriState.msoTrue);
            this.SetTextRange(slide.Shapes[2].TextFrame.TextRange, content, contentSize, MsoTriState.msoTrue);
            if (isCenter)
            {
                slide.Shapes[2].TextFrame.TextRange.ParagraphFormat.Alignment = PpParagraphAlignment.ppAlignCenter;
            }

            return slide;
        }

        protected IEnumerable<Slide> CreateContentSlideWithTitleContent(string title, float titleSize, IEnumerable<string> paragraphs, float contentSize, bool isCenter = false)
        {
            var lstSlide = new List<Slide>();

            var slide = this.CreateContentSlideWithTitleContent(title, titleSize, string.Empty, contentSize, isCenter);
            lstSlide.Add(slide);
            var textRange = slide.Shapes[2].TextFrame.TextRange;

            var lstLine = new List<string>();
            foreach (var p in paragraphs)
            {
                if (280 < textRange.BoundHeight)
                {
                    slide = this.CreateContentSlideWithTitleContent(title, titleSize, string.Empty, contentSize, isCenter);
                    lstSlide.Add(slide);
                    textRange = slide.Shapes[2].TextFrame.TextRange;
                    lstLine.Clear();
                }

                lstLine.Add(p);
                var content = string.Join(Environment.NewLine, lstLine);
                textRange.Text = content;
            }

            return lstSlide;
        }

        protected Microsoft.Office.Interop.PowerPoint.Shape AttachBackground(Slide slide, string filename, float scaleWidth = 1, float scaleHeight = 1)
        {
            var shape = slide.Shapes.AddPicture(filename, MsoTriState.msoFalse, MsoTriState.msoTrue, 0, 0);
            shape.Top = 0;
            shape.Left = 0;
            shape.LockAspectRatio = MsoTriState.msoFalse;
            shape.ScaleWidth(scaleWidth, MsoTriState.msoTrue);
            shape.ScaleHeight(scaleHeight, MsoTriState.msoTrue);
            shape.ZOrder(MsoZOrderCmd.msoSendToBack);

            return shape;
        }

        private void SetTextRange(TextRange textRange, string text, float size, MsoTriState bold)
        {
            textRange.Text = text;

            textRange.Font.NameAscii = this._nameAscii;
            textRange.Font.NameFarEast = this._nameFarEast;
            textRange.Font.Size = size;
            textRange.Font.Bold = bold;
            textRange.Font.Color.RGB = Color.White.ToArgb();
        }
    }
}
