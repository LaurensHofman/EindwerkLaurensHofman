namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class locenumv : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LocalizedEnumValues",
                c => new
                    {
                        EnumerationID = c.Int(nullable: false),
                        LanguageID = c.Int(nullable: false),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => new { t.EnumerationID, t.LanguageID })
                .ForeignKey("dbo.Languages", t => t.LanguageID, cascadeDelete: true)
                .ForeignKey("dbo.SpecificationEnums", t => t.EnumerationID, cascadeDelete: true)
                .Index(t => t.EnumerationID)
                .Index(t => t.LanguageID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocalizedEnumValues", "EnumerationID", "dbo.SpecificationEnums");
            DropForeignKey("dbo.LocalizedEnumValues", "LanguageID", "dbo.Languages");
            DropIndex("dbo.LocalizedEnumValues", new[] { "LanguageID" });
            DropIndex("dbo.LocalizedEnumValues", new[] { "EnumerationID" });
            DropTable("dbo.LocalizedEnumValues");
        }
    }
}
