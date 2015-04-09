using System;
using System.Data.Entity;

namespace StandUpTimer.Web.Models
{
    public class Status
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public Position Position { get; set; }
    }

    public enum Position
    {
        Standing,
        Sitting,
        Inactive
    }

    public class StatusContext : DbContext
    {
        public DbSet<Status> Statuses { get; set; }
    }
}