// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChurchFactory.cs" company="Church">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Defines the interface for Churchs.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LyricsBuilder
    {
        public IEnumerable<Hymn> Make(string plainText)
        {
            const string REFRAIN_MARK = "===Refrain===";
            const string PARTIAL_MARK = "===Partial===";

            string SONG_SEPARATOR = Environment.NewLine + Environment.NewLine + Environment.NewLine;
            string SECTION_SEPARATOR = Environment.NewLine + Environment.NewLine;
            var songs = plainText.Split(new string[] { SONG_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var s in songs)
            {
                var sections = s.Split(new string[] { SECTION_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => this.RemoveLeadingNewLine(line));

                var names = sections.First().Split('@');
                var name = names[0];
                var source = (1 < names.Length) ? names[1] : string.Empty;
                var lstSection = new List<LyricsSection>();
                for (int loop = 0; loop < sections.Count(); loop++)
                {
                    if (loop == 0)
                    {
                        continue;
                    }
                    LyricsSectionType type = LyricsSectionType.Main;
                    var current = sections.ElementAt(loop).Trim();
                    if (current.StartsWith(REFRAIN_MARK))
                    {
                        type = LyricsSectionType.Refrain;
                        current = this.RemoveLeadingNewLine(current.Substring(REFRAIN_MARK.Length));
                    }
                    else if (current.StartsWith(PARTIAL_MARK))
                    {
                        type = LyricsSectionType.Partial;
                        current = this.RemoveLeadingNewLine(current.Substring(PARTIAL_MARK.Length));
                    }
                    var lines = current.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    var section = new LyricsSection
                    {
                        Type = type,
                        Text = lines
                    };
                    lstSection.Add(section);
                }

                yield return new Hymn
                {
                    Name = name,
                    Lyrics = Utilities.ToJson(lstSection.ToArray()),
                    Source = source,
                    Links = string.Empty
                    //Links = Utilities.ToJson(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase))
                };
            }
        }
        private string RemoveLeadingNewLine(string line)
        {
            return line.TrimStart('\r', '\n');
        }
    }
}