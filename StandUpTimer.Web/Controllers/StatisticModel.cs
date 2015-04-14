using System.Collections.Generic;
using StandUpTimer.Web.Statistics;

namespace StandUpTimer.Web.Controllers
{
    public class StatisticModel
    {
        public List<GanttStatus> Statuses { get; set; }
        public List<string> Days { get; set; }
    }
}