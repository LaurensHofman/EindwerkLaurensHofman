namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class locprod : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LocalizedProducts",
                c => new
                    {
                        ProductID = c.Int(nullable: false),
                        LanguageID = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProductID, t.LanguageID })
                .ForeignKey("dbo.Languages", t => t.LanguageID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID)
                .Index(t => t.LanguageID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocalizedProducts", "ProductID", "dbo.Products");
            DropForeignKey("dbo.LocalizedProducts", "LanguageID", "dbo.Languages");
            DropIndex("dbo.LocalizedProducts", new[] { "LanguageID" });
            DropIndex("dbo.LocalizedProducts", new[] { "ProductID" });
            DropTable("dbo.LocalizedProducts");
        }
    }
}
