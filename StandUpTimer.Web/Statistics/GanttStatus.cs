using StandUpTimer.Web.Models;

namespace StandUpTimer.Web.Statistics
{
    public class GanttStatus
    {
        public DeskState DeskState { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Day { get; set; }
    }
}