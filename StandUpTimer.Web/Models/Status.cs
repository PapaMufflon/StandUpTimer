using System;

namespace StandUpTimer.Web.Models
{
    public class Status
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public DeskState DeskState { get; set; }
    }

    public enum DeskState
    {
        Standing,
        Sitting,
        Inactive
    }
}