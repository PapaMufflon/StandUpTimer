using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using StandUpTimer.Web.Models;
using Status = StandUpTimer.Web.Models.Status;

namespace StandUpTimer.Web.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public ActionResult Index([Bind(Include = "DateTime,DeskState")] Contract.Status status)
        {
            db.Statuses.Add(status.ToModel());
            db.SaveChanges();

            return null;
        }

        [HttpGet]
        // GET: Statistics
        public ActionResult Index()
        {
            var statuses = db.Statuses.ToList();
            statuses.AddRange(new List<Status>
            {
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 8, 15, 0),
                    DeskState = DeskState.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 9, 17, 0),
                    DeskState = DeskState.Standing
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 9, 37, 0),
                    DeskState = DeskState.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 11, 0, 0),
                    DeskState = DeskState.Standing
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 11, 20, 0),
                    DeskState = DeskState.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 12, 15, 0),
                    DeskState = DeskState.Inactive
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 12, 50, 0),
                    DeskState = DeskState.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 14, 0, 0),
                    DeskState = DeskState.Standing
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 14, 20, 0),
                    DeskState = DeskState.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 15, 25, 0),
                    DeskState = DeskState.Standing
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 15, 45, 0),
                    DeskState = DeskState.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 17, 5, 0),
                    DeskState = DeskState.Inactive
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 8, 8, 15, 0),
                    DeskState = DeskState.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 8, 9, 17, 0),
                    DeskState = DeskState.Standing
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 8, 9, 37, 0),
                    DeskState = DeskState.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 8, 11, 0, 0),
                    DeskState = DeskState.Standing
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 8, 11, 20, 0),
                    DeskState = DeskState.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 8, 12, 15, 0),
                    DeskState = DeskState.Inactive
                }
            });

            var ganttStatuses = statuses.Ganttisize();

            return View(new StatisticModel
            {
                Statuses = ganttStatuses,
                Days = ganttStatuses.Select(x => x.Day).Distinct().ToList()
            });
        }
    }

    public static class GanttificationExtension
    {
        public static List<GanttStatus> Ganttisize(this IList<Status> statuses)
        {
            var result = new List<GanttStatus>();

            for (int index = 1; index < statuses.Count; index++)
            {
                var status = statuses[index];
                var previousStatus = statuses[index - 1];

                result.Add(new GanttStatus
                {
                    DeskState = previousStatus.DeskState,
                    StartDate = DateTime.Today.Add(previousStatus.DateTime.TimeOfDay).ToString(Contract.Status.DateTimeFormat),
                    EndDate = DateTime.Today.Add(status.DateTime.TimeOfDay).ToString(Contract.Status.DateTimeFormat),
                    Day = ToReadableDay(previousStatus.DateTime)
                });
            }

            return result;
        }

        private static string ToReadableDay(DateTime dateTime)
        {
            if (dateTime.Date.Equals(DateTime.Today))
                return "Heute";

            if (dateTime.Date.Equals(DateTime.Today.AddDays(-1)))
                return "Gestern";

            return dateTime.Date.ToShortDateString();
        }
    }

    public class GanttStatus
    {
        public DeskState DeskState { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Day { get; set; }
    }

    public class StatisticModel
    {
        public List<GanttStatus> Statuses { get; set; }
        public List<string> Days { get; set; }
    }

    public static class StatusConversionExtension
    {
        public static Status ToModel(this Contract.Status status)
        {
            return new Status
            {
                DateTime = DateTime.ParseExact(status.DateTime, Contract.Status.DateTimeFormat, CultureInfo.InvariantCulture),
                DeskState = status.DeskState.ToModel()
            };
        }

        public static DeskState ToModel(this Contract.DeskState deskState)
        {
            switch (deskState)
            {
                case Contract.DeskState.Standing:
                    return DeskState.Standing;
                case Contract.DeskState.Sitting:
                    return DeskState.Sitting;
                case Contract.DeskState.Inactive:
                    return DeskState.Inactive;
                default:
                    throw new ArgumentOutOfRangeException("deskState");
            }
        }
    }
}