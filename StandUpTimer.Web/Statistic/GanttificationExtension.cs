using System;
using System.Collections.Generic;
using System.Linq;
using StandUpTimer.Common;
using StandUpTimer.Web.Models;

namespace StandUpTimer.Web.Statistic
{
    public static class GanttificationExtension
    {
        public static List<GanttStatus> Ganttisize(this IList<Status> statuses)
        {
            var result = new List<GanttStatus>();

            if (statuses.Count == 0)
                return result;

            var sortedStatuses = statuses.ToList();
            sortedStatuses.Sort((x, y) => x.DateTime.CompareTo(y.DateTime));

            for (int index = 1; index < sortedStatuses.Count; index++)
                TryAddGanttStatus(sortedStatuses[index - 1], sortedStatuses[index].DateTime, result);

            // using DateTime.Now here on the server is actually wrong (may not be in the same time zone as the user)
            // but as it will only affect the current running state which is over 24 hours, we will use the shortcut here
            // and do not handle all the stuff on the client but on the server.
            if (IsValidGanttStatus(sortedStatuses.Last(), TestableDateTime.Now))
                AddLastGanttStatus(sortedStatuses.Last(), result);

            return result;
        }

        private static void TryAddGanttStatus(Status previousStatus, DateTime currentStatusPosition, List<GanttStatus> result)
        {
            if (IsValidGanttStatus(previousStatus, currentStatusPosition))
            {
                if (SpansOverTwoDays(previousStatus.DateTime, currentStatusPosition))
                {
                    AddGanttStatus(previousStatus, currentStatusPosition.Date.AddSeconds(-1), result);
                    AddGanttStatus(previousStatus.WithDate(currentStatusPosition.Date), currentStatusPosition, result);
                }
                else
                    AddGanttStatus(previousStatus, currentStatusPosition, result);
            }
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

        private static bool SpansOverTwoDays(DateTime start, DateTime end)
        {
            return !start.Date.Equals(end.Date);
        }

        private static void AddGanttStatus(Status previousStatus, DateTime currentStatusPosition, List<GanttStatus> result)
        {
            result.Add(new GanttStatus
            {
                DeskState = previousStatus.DeskState,
                StartDate = TestableDateTime.Today.Add(previousStatus.DateTime.TimeOfDay).ToString(Contract.Status.DateTimeFormat),
                EndDate = TestableDateTime.Today.Add(currentStatusPosition.TimeOfDay).ToString(Contract.Status.DateTimeFormat),
                Day = ToReadableDay(previousStatus.DateTime)
            });
        }

        private static void AddLastGanttStatus(Status previousStatus, List<GanttStatus> result)
        {
            result.Add(new GanttStatus
            {
                DeskState = previousStatus.DeskState,
                StartDate = TestableDateTime.Today.Add(previousStatus.DateTime.TimeOfDay).ToString(Contract.Status.DateTimeFormat),
                EndDate = "now",
                Day = ToReadableDay(previousStatus.DateTime)
            });
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