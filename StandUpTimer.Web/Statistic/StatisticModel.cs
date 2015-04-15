using System.Collections.Generic;

namespace StandUpTimer.Web.Statistic
{
    public class StatisticModel
    {
        public List<GanttStatus> Statuses { get; set; }
        public List<string> Days { get; set; }
    }
}