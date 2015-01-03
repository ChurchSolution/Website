namespace Church.Model
{
    using Church;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class NorthVirginiaChineseBaptistChurchCnBulletinBuilder : BulletinTextBuilder
    {
        char[] Commas = new[] { ',', '，', '、' };
        char[] Semicollons = new[] { ';', '；' };
        char[] Collons = new[] { ':', '：' };

        const string WordFromPasterSectionKey = "牧者的话  WORD FROM  PASTOR";
        const string LastWeekDataSectionKey = "上周主日出席人数：";
        const string NextWeekServicesSectionKey = "下周主日崇拜事奉分工";
        const string HeaderSectionKey = "北  维  州  华  人  浸  信  会";
        const string WorshipProgramSectionKey = "主 日 崇 拜";
        const string ServicesSectionKey = "主日崇拜事奉分工";
        const string MemorizedVersesSectionKey = "记忆经文  MEMORY VERSE OF THE WEEK";
        const string AnnouncementsSectionKey = "报告事项   ANNOUNCEMENTS";
        const string PrayerRequestsSectionKey = "代祷事项  PRAYER REQUESTS";
        const string GroupLeadersSectionKey = "事工组负责人  GROUP LEADERS";
        const string ActivitiesSectionKey = "教会活动  ACTIVITIES";
        const string FamilyWorshipSectionKey = "本周家庭崇拜";
        const string SermonCompendiumSectionKey = "证道大纲：";
        const string SermonCompendiumSectionKey2 = "见证分享：";
        const string WeeklyReadingSectionKey = "本周读经表";
        const string ChurchOfficeSectionKey = "* 教会办公室地址";

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
                    case "儿童":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '主日儿童'.");
                        lastWeekData.Add("主日儿童", ParseNumber(items[1].Trim()));
                        break;
                }
            });
            this.ProcessColonTable(new List<string>(new string[] { lines.Skip(1).First() }), items =>
            {
                switch (items[0].Trim())
                {
                    case "成人":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '周间成人'.");
                        lastWeekData.Add("周间成人", ParseNumber(items[1].Trim()));
                        break;
                    case "儿童":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '周间儿童'.");
                        lastWeekData.Add("周间儿童", ParseNumber(items[1].Trim()));
                        break;
                }
            });
            this.ProcessColonTable(lines.Skip(2), items =>
            {
                switch (items[0].Trim())
                {
                    case "上周总奉献":
                    case "上两周总奉献":
                    case "前周总奉献":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '奉献总计'.");
                        lastWeekData.Add("奉献总计", ParseMoney(items[1].Trim()));
                        break;
                    case "一般":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '一般奉献'.");
                        lastWeekData.Add("一般奉献", ParseMoney(items[1].Trim()));
                        break;
                    case "建堂":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '建堂奉献'.");
                        lastWeekData.Add("建堂奉献", ParseMoney(items[1].Trim()));
                        break;
                    case "爱心":
                        ExceptionUtilities.ThrowFormatExceptionIfFalse(2 == items.Length, "Could not find the number for '爱心奉献'.");
                        lastWeekData.Add("爱心奉献", ParseMoney(items[1].Trim()));
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
            var res = this.ProcessWordTable(lines.Skip(1), s => s, 2, cells =>
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
                    case "讲员":
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

            var worshipProgram = new List<NorthVirginiaChineseBaptistChurchBulletin.ProgramItem>();
            foreach (string l in lines.Skip(2).Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("*")))
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
                if ("崇拜聚会点".Equals(location, StringComparison.OrdinalIgnoreCase))
                {
                    map = "<a target=\"_blank\" href=\"https://maps.google.com/maps?q=5035+Sideburn+Rd,+Fairfax,+VA+22032&hl=en&sll=38.003385,-79.420925&sspn=3.774088,8.426514&hnear=5035+Sideburn+Rd,+Fairfax,+Virginia+22032&t=m&z=16\">地图</a>";
                }
                else if ("教会办公室".Equals(location, StringComparison.OrdinalIgnoreCase))
                {
                    map = "<a target=\"_blank\" href=\"https://maps.google.com/maps?q=11094+Lee+Highway,+A103,+Fairfax,+VA+22030&hl=en&sll=38.818833,-77.309889&sspn=0.033437,0.064459&hnear=11094+Lee+Hwy,+Fairfax,+Virginia+22030&t=m&z=14\">地图</a>";
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
            //诗歌: 《主，我敬拜你》，经文：可10：6-9
            //网上播放:	 http://www.youtube.com/watch?v=WsmnClrUBrY&feature=related
            var firstLine = lines.Skip(1).First();
            var cells = firstLine.Split(new string[] { "》，经文：" }, StringSplitOptions.None);
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
                    addItem(c.Split(Collons));
                }
            });
        }
    }
}