namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initTest1ToMany1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DesktopUsers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IsAdmin = c.Boolean(nullable: false),
                        VerifiedByAdmin = c.Boolean(nullable: false),
                        LastName = c.String(nullable: false),
                        FirstName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Username = c.String(nullable: false),
                        EncryptedPassword = c.String(nullable: false),
                        Salt = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        ModifiedAt = c.DateTime(nullable: false),
                        DeletedAt = c.DateTime(),
                        PreferredLanguage_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Languages", t => t.PreferredLanguage_ID)
                .Index(t => t.PreferredLanguage_ID);
            
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LocalName = c.String(nullable: false, maxLength: 255),
                        DutchName = c.String(nullable: false, maxLength: 255),
                        EnglishName = c.String(nullable: false, maxLength: 255),
                        ISO = c.String(nullable: false, maxLength: 3),
                        IsDesktopLanguage = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        ModifiedAt = c.DateTime(nullable: false),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.ISO, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DesktopUsers", "PreferredLanguage_ID", "dbo.Languages");
            DropIndex("dbo.Languages", new[] { "ISO" });
            DropIndex("dbo.DesktopUsers", new[] { "PreferredLanguage_ID" });
            DropTable("dbo.Languages");
            DropTable("dbo.DesktopUsers");
        }
    }
}
