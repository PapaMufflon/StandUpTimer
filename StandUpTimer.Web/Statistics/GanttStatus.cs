using StandUpTimer.Web.Models;

namespace StandUpTimer.Web.Statistics
{
    public class GanttStatus
    {
        public DeskState DeskState { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Day { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1} - {2}: {3}", Day, StartDate, EndDate, DeskState);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GanttStatus) obj);
        }

        protected bool Equals(GanttStatus other)
        {
            return DeskState == other.DeskState && string.Equals(StartDate, other.StartDate) && string.Equals(EndDate, other.EndDate) && string.Equals(Day, other.Day);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)DeskState;
                hashCode = (hashCode * 397) ^ (StartDate != null ? StartDate.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EndDate != null ? EndDate.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Day != null ? Day.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}