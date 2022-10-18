namespace Hrmis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class checking : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo._Role",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo._UserRole",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo._Role", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo._User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo._User",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DivisionID = c.String(),
                        DistrictID = c.String(),
                        TehsilID = c.String(),
                        hfmiscode = c.String(),
                        LevelID = c.Int(nullable: false),
                        DesigCode = c.Int(),
                        isActive = c.Boolean(nullable: false),
                        HfmisCodeNew = c.String(),
                        CreationDate = c.DateTime(),
                        CreatedBy = c.Decimal(precision: 18, scale: 2),
                        ModifiedDate = c.DateTime(),
                        ModifiedBy = c.Decimal(precision: 18, scale: 2),
                        UserDetail = c.String(),
                        responsibleuser = c.String(),
                        hashynoty = c.String(),
                        HfTypeCode = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo._UserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo._User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo._UserLogin",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo._User", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo._UserRole", "UserId", "dbo._User");
            DropForeignKey("dbo._UserLogin", "UserId", "dbo._User");
            DropForeignKey("dbo._UserClaim", "UserId", "dbo._User");
            DropForeignKey("dbo._UserRole", "RoleId", "dbo._Role");
            DropIndex("dbo._UserLogin", new[] { "UserId" });
            DropIndex("dbo._UserClaim", new[] { "UserId" });
            DropIndex("dbo._User", "UserNameIndex");
            DropIndex("dbo._UserRole", new[] { "RoleId" });
            DropIndex("dbo._UserRole", new[] { "UserId" });
            DropIndex("dbo._Role", "RoleNameIndex");
            DropTable("dbo._UserLogin");
            DropTable("dbo._UserClaim");
            DropTable("dbo._User");
            DropTable("dbo._UserRole");
            DropTable("dbo._Role");
        }
    }
}
