namespace Church.Models
{
    using System.Diagnostics;

    public class NorthVirginiaChineseBaptistChurchWeeklyRecord : IWeeklyRecord
    {
        public string Name { get; internal set; }
        public string FileUrl { get; internal set; }

        public int NumberOfAdults { get; internal set; }
        public int NumberOfChildren { get; internal set; }
        public int NumberOfAdultsInWeekday { get; internal set; }
        public int NumberOfChildrenInWeekday { get; internal set; }
        public float TotalContribution { get; internal set; }
        public float GeneralContribution { get; internal set; }
        public float ChurchBuilding { get; internal set; }
        public float BeyondOutreachFoundation { get; internal set; }

        public IWeeklyRecord Add(IWeeklyRecord record)
        {
            Trace.Assert(record is NorthVirginiaChineseBaptistChurchWeeklyRecord);

            var r = record as NorthVirginiaChineseBaptistChurchWeeklyRecord;
            this.NumberOfAdults += r.NumberOfAdults;
            this.NumberOfChildren += r.NumberOfChildren;
            this.NumberOfAdultsInWeekday += r.NumberOfAdultsInWeekday;
            this.NumberOfChildrenInWeekday += r.NumberOfChildrenInWeekday;
            this.TotalContribution += r.TotalContribution;
            this.GeneralContribution += r.GeneralContribution;
            this.ChurchBuilding += r.ChurchBuilding;
            this.BeyondOutreachFoundation += r.BeyondOutreachFoundation;

            return this;
        }

        public IWeeklyRecord DivideBy(int num)
        {
            Trace.Assert(num >= 0, "Could not divide by negative number");
            if (0 < num)
            {
                this.NumberOfAdults /= num;
                this.NumberOfChildren /= num;
                this.NumberOfAdultsInWeekday /= num;
                this.NumberOfChildrenInWeekday /= num;
                this.TotalContribution /= num;
                this.GeneralContribution /= num;
                this.ChurchBuilding /= num;
                this.BeyondOutreachFoundation /= num;
            }

            return this;
        }
    }
}
