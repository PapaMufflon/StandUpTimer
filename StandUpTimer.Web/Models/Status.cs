﻿using System;

namespace StandUpTimer.Web.Models
{
    public class Status
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime DateTime { get; set; }
        public DeskState DeskState { get; set; }

        public Status WithDate(DateTime date)
        {
            return new Status
            {
                Id = Id,
                Username = Username,
                DateTime = date,
                DeskState = DeskState
            };
        }
    }

    public enum DeskState
    {
        Standing,
        Sitting,
        Inactive
    }
}