namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class onetomanyprodimg : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductImages", "Product_ID", "dbo.Products");
            DropIndex("dbo.ProductImages", new[] { "Product_ID" });
            RenameColumn(table: "dbo.ProductImages", name: "Product_ID", newName: "ProductID");
            AlterColumn("dbo.ProductImages", "ProductID", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductImages", "ProductID");
            AddForeignKey("dbo.ProductImages", "ProductID", "dbo.Products", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductImages", "ProductID", "dbo.Products");
            DropIndex("dbo.ProductImages", new[] { "ProductID" });
            AlterColumn("dbo.ProductImages", "ProductID", c => c.Int());
            RenameColumn(table: "dbo.ProductImages", name: "ProductID", newName: "Product_ID");
            CreateIndex("dbo.ProductImages", "Product_ID");
            AddForeignKey("dbo.ProductImages", "Product_ID", "dbo.Products", "ID");
        }
    }
}
