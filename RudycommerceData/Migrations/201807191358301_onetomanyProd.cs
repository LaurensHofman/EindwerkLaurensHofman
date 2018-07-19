namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onetomanyProd : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "Brand_ID", "dbo.Brands");
            DropForeignKey("dbo.Products", "Category_ID", "dbo.Categories");
            DropIndex("dbo.Products", new[] { "Brand_ID" });
            DropIndex("dbo.Products", new[] { "Category_ID" });
            RenameColumn(table: "dbo.Products", name: "Brand_ID", newName: "BrandID");
            RenameColumn(table: "dbo.Products", name: "Category_ID", newName: "CategoryID");
            AlterColumn("dbo.Products", "BrandID", c => c.Int(nullable: false));
            AlterColumn("dbo.Products", "CategoryID", c => c.Int(nullable: false));
            CreateIndex("dbo.Products", "CategoryID");
            CreateIndex("dbo.Products", "BrandID");
            AddForeignKey("dbo.Products", "BrandID", "dbo.Brands", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Products", "CategoryID", "dbo.Categories", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.Products", "BrandID", "dbo.Brands");
            DropIndex("dbo.Products", new[] { "BrandID" });
            DropIndex("dbo.Products", new[] { "CategoryID" });
            AlterColumn("dbo.Products", "CategoryID", c => c.Int());
            AlterColumn("dbo.Products", "BrandID", c => c.Int());
            RenameColumn(table: "dbo.Products", name: "CategoryID", newName: "Category_ID");
            RenameColumn(table: "dbo.Products", name: "BrandID", newName: "Brand_ID");
            CreateIndex("dbo.Products", "Category_ID");
            CreateIndex("dbo.Products", "Brand_ID");
            AddForeignKey("dbo.Products", "Category_ID", "dbo.Categories", "ID");
            AddForeignKey("dbo.Products", "Brand_ID", "dbo.Brands", "ID");
        }
    }
}
