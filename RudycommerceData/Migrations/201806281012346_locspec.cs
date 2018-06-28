namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class locspec : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LocalizedSpecifications",
                c => new
                    {
                        PropertyID = c.Int(nullable: false),
                        LanguageID = c.Int(nullable: false),
                        LookupName = c.String(nullable: false),
                        AdviceDescription = c.String(),
                    })
                .PrimaryKey(t => new { t.PropertyID, t.LanguageID })
                .ForeignKey("dbo.Languages", t => t.LanguageID, cascadeDelete: true)
                .ForeignKey("dbo.Specifications", t => t.PropertyID, cascadeDelete: true)
                .Index(t => t.PropertyID)
                .Index(t => t.LanguageID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocalizedSpecifications", "PropertyID", "dbo.Specifications");
            DropForeignKey("dbo.LocalizedSpecifications", "LanguageID", "dbo.Languages");
            DropIndex("dbo.LocalizedSpecifications", new[] { "LanguageID" });
            DropIndex("dbo.LocalizedSpecifications", new[] { "PropertyID" });
            DropTable("dbo.LocalizedSpecifications");
        }
    }
}
