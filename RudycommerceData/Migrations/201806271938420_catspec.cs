namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class catspec : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CategorySpecifications",
                c => new
                    {
                        CategoryID = c.Int(nullable: false),
                        SpecificationID = c.Int(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CategoryID, t.SpecificationID })
                .ForeignKey("dbo.Categories", t => t.CategoryID, cascadeDelete: true)
                .ForeignKey("dbo.Specifications", t => t.SpecificationID, cascadeDelete: true)
                .Index(t => t.CategoryID)
                .Index(t => t.SpecificationID);
            
            CreateTable(
                "dbo.LocalizedCategories",
                c => new
                    {
                        CategoryID = c.Int(nullable: false),
                        LanguageID = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        PluralName = c.String(nullable: false),
                    })
                .PrimaryKey(t => new { t.CategoryID, t.LanguageID })
                .ForeignKey("dbo.Categories", t => t.CategoryID, cascadeDelete: true)
                .ForeignKey("dbo.Languages", t => t.LanguageID, cascadeDelete: true)
                .Index(t => t.CategoryID)
                .Index(t => t.LanguageID);
            
            CreateTable(
                "dbo.SpecificationEnums",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Specification_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Specifications", t => t.Specification_ID)
                .Index(t => t.Specification_ID);
            
            CreateTable(
                "dbo.Specifications",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IsMultilingual = c.Boolean(nullable: false),
                        IsBool = c.Boolean(nullable: false),
                        IsEnumeration = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        ModifiedAt = c.DateTime(nullable: false),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SpecificationEnums", "Specification_ID", "dbo.Specifications");
            DropForeignKey("dbo.CategorySpecifications", "SpecificationID", "dbo.Specifications");
            DropForeignKey("dbo.LocalizedCategories", "LanguageID", "dbo.Languages");
            DropForeignKey("dbo.LocalizedCategories", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.CategorySpecifications", "CategoryID", "dbo.Categories");
            DropIndex("dbo.SpecificationEnums", new[] { "Specification_ID" });
            DropIndex("dbo.LocalizedCategories", new[] { "LanguageID" });
            DropIndex("dbo.LocalizedCategories", new[] { "CategoryID" });
            DropIndex("dbo.CategorySpecifications", new[] { "SpecificationID" });
            DropIndex("dbo.CategorySpecifications", new[] { "CategoryID" });
            DropTable("dbo.Specifications");
            DropTable("dbo.SpecificationEnums");
            DropTable("dbo.LocalizedCategories");
            DropTable("dbo.CategorySpecifications");
        }
    }
}
