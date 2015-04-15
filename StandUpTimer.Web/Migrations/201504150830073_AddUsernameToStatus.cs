using System.Data.Entity.Migrations;

namespace StandUpTimer.Web.Migrations
{
    public partial class AddUsernameToStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Status", "Username", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Status", "Username");
        }
    }
}