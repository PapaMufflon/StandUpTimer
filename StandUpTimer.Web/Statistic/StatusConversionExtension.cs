using System;
using System.Globalization;
using StandUpTimer.Web.Models;

namespace StandUpTimer.Web.Statistic
{
    public static class StatusConversionExtension
    {
        public static Status ToModel(this Contract.Status status, string username)
        {
            return new Status
            {
                Username = username,
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