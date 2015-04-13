using System;
using System.Data.Entity;
using StandUpTimer.Web.Contract;

namespace StandUpTimer.Web.Models
{
    public class Status
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public DeskState DeskState { get; set; }
    }

    public class StatusContext : DbContext
    {
        public DbSet<Status> Statuses { get; set; }
    }
}