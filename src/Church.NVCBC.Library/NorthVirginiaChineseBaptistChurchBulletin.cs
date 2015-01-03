namespace Church.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    public class NorthVirginiaChineseBaptistChurchBulletin : IWeeklyBulletinProperties
    {
        public class ServiceItem
        {
            public string Name { get; private set; }

            public IEnumerable<string> People { get; internal set; }

            public IEnumerable<string> PeopleForNext { get; internal set; }

            public static ServiceItem Create(string name)
            {
                return new ServiceItem { Name = name, People = Enumerable.Empty<string>(), PeopleForNext = Enumerable.Empty<string>() };
            }
        }

        public class ProgramItem
        {
            public string Name { get; private set; }

            public string Value { get; private set; }

            public string NameInEnglish { get; private set; }

            public static ProgramItem Create(string name, string value, string nameInEnglish)
            {
                return new ProgramItem { Name = name, Value = value, NameInEnglish = nameInEnglish };
            }
        }

        public class ActivityItem
        {
            public string Name { get; private set; }

            public string Time { get; private set; }

            public string LocationOrContact { get; private set; }

            public string LocationMap { get; private set; }

            public string Phone { get; private set; }

            public static ActivityItem Create(string name, string time, string locationOrContact, string locationMap, string phone)
            {
                return new ActivityItem
                {
                    Name = name,
                    Time = time,
                    LocationOrContact = locationOrContact,
                    LocationMap = locationMap,
                    Phone = phone
                };
            }
        }

        public class FamilyWorshipItem
        {
            public string PraiseName { get; private set; }

            public string PraiseUri { get; private set; }

            public string Verse { get; private set; }

            public static FamilyWorshipItem Create(string praiseName, string praiseUri, string verse)
            {
                return new FamilyWorshipItem { PraiseName = praiseName, PraiseUri = praiseUri, Verse = verse };
            }
        }

        public DateTime Date { get; internal set; }

        public IEnumerable<string> WordFromPastor { get; internal set; }

        public Dictionary<string, decimal> LastWeekData { get; internal set; }

        public Dictionary<string, ServiceItem> Services { get; internal set; }

        public IEnumerable<ProgramItem> WorshipProgram { get; internal set; }

        public string Speaker { get; internal set; }

        public IEnumerable<string> MemorizedVerses { get; internal set; }

        public IEnumerable<string> Announcements { get; internal set; }

        public IEnumerable<string> PrayerRequests { get; internal set; }

        public IEnumerable<ActivityItem> Activities { get; internal set; }

        public FamilyWorshipItem FamilyWorship { get; internal set; }

        public IEnumerable<string> SermonCompendium { get; internal set; }
        
        public string PastorName { get; internal set; }

        public string MessageTitle { get; internal set; }

        public string MessageVerseAbbr { get; internal set; }

        public bool Verify()
        {
            return null != this.WorshipProgram
                && null != this.WordFromPastor
                && null != this.FamilyWorship
                && null != this.Services
                && null != this.Speaker;
        }
    }
}
