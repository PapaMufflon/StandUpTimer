using System;

namespace StandUpTimer.Web.Contract
{
    public class Status
    {
        public DateTime DateTime { get; set; }
        public DeskState DeskState { get; set; }
    }
}