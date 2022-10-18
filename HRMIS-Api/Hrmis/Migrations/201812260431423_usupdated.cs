namespace Hrmis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usupdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo._User", "isUpdated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo._User", "isUpdated");
        }
    }
}
