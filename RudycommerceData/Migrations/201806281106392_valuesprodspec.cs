namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class valuesprodspec : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Values_ProductSpecifications",
                c => new
                    {
                        ProductID = c.Int(nullable: false),
                        SpecificationID = c.Int(nullable: false),
                        LanguageID = c.Int(nullable: false),
                        Value = c.String(),
                        EnumValueID = c.Int(),
                        EnumerationValue_ID = c.Int(),
                    })
                .PrimaryKey(t => new { t.ProductID, t.SpecificationID, t.LanguageID })
                .ForeignKey("dbo.Specifications", t => t.SpecificationID, cascadeDelete: true)
                .ForeignKey("dbo.SpecificationEnums", t => t.EnumerationValue_ID)
                .ForeignKey("dbo.Languages", t => t.LanguageID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .Index(t => t.ProductID)
                .Index(t => t.SpecificationID)
                .Index(t => t.LanguageID)
                .Index(t => t.EnumerationValue_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Values_ProductSpecifications", "ProductID", "dbo.Products");
            DropForeignKey("dbo.Values_ProductSpecifications", "LanguageID", "dbo.Languages");
            DropForeignKey("dbo.Values_ProductSpecifications", "EnumerationValue_ID", "dbo.SpecificationEnums");
            DropForeignKey("dbo.Values_ProductSpecifications", "SpecificationID", "dbo.Specifications");
            DropIndex("dbo.Values_ProductSpecifications", new[] { "EnumerationValue_ID" });
            DropIndex("dbo.Values_ProductSpecifications", new[] { "LanguageID" });
            DropIndex("dbo.Values_ProductSpecifications", new[] { "SpecificationID" });
            DropIndex("dbo.Values_ProductSpecifications", new[] { "ProductID" });
            DropTable("dbo.Values_ProductSpecifications");
        }
    }
}
