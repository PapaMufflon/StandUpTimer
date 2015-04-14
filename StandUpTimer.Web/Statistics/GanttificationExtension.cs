using System;
using System.Collections.Generic;
using StandUpTimer.Common;
using StandUpTimer.Web.Models;

namespace StandUpTimer.Web.Statistics
{
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
                    StartDate = TestableDateTime.Today.Add(previousStatus.DateTime.TimeOfDay).ToString(Contract.Status.DateTimeFormat),
                    EndDate = TestableDateTime.Today.Add(status.DateTime.TimeOfDay).ToString(Contract.Status.DateTimeFormat),
                    Day = ToReadableDay(previousStatus.DateTime)
                });
            }

            return result;
        }

        private static string ToReadableDay(DateTime dateTime)
        {
            if (dateTime.Date.Equals(TestableDateTime.Today))
                return "Heute";

            if (dateTime.Date.Equals(TestableDateTime.Today.AddDays(-1)))
                return "Gestern";

            return dateTime.Date.ToShortDateString();
        }
    }
}