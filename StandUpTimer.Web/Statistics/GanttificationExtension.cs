using System;
using System.Collections.Generic;
using System.Linq;
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

                if (!IsValidGanttStatus(previousStatus, status.DateTime))
                    continue;

                result.Add(new GanttStatus
                {
                    DeskState = previousStatus.DeskState,
                    StartDate = TestableDateTime.Today.Add(previousStatus.DateTime.TimeOfDay).ToString(Contract.Status.DateTimeFormat),
                    EndDate = TestableDateTime.Today.Add(status.DateTime.TimeOfDay).ToString(Contract.Status.DateTimeFormat),
                    Day = ToReadableDay(previousStatus.DateTime)
                });
            }

            var lastStatus = statuses.Last();

            if (IsValidGanttStatus(lastStatus, TestableDateTime.Now))
            {
                result.Add(new GanttStatus
                {
                    DeskState = lastStatus.DeskState,
                    StartDate = TestableDateTime.Today.Add(lastStatus.DateTime.TimeOfDay).ToString(Contract.Status.DateTimeFormat),
                    EndDate = TestableDateTime.Now.ToString(Contract.Status.DateTimeFormat),
                    Day = ToReadableDay(lastStatus.DateTime)
                });
            }

            return result;
        }

        private static bool IsValidGanttStatus(Status previousStatus, DateTime currentStatusPosition)
        {
            return previousStatus.DeskState != DeskState.Inactive &&
                   !StateLastsTooLong(previousStatus, currentStatusPosition);
        }

        private static bool StateLastsTooLong(Status previousStatus, DateTime currentStatusPosition)
        {
            return currentStatusPosition.Subtract(previousStatus.DateTime) > TimeSpan.FromDays(1);
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