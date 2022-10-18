namespace Hrmis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cnincProfileId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo._User", "Cnic", c => c.String());
            AddColumn("dbo._User", "ProfileId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo._User", "ProfileId");
            DropColumn("dbo._User", "Cnic");
        }
    }
}
