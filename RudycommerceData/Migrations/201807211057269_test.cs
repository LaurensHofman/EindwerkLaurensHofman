namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductImages", "ProductID", "dbo.Products");
            DropIndex("dbo.ProductImages", new[] { "ProductID" });
            AlterColumn("dbo.ProductImages", "ProductID", c => c.Int());
            CreateIndex("dbo.ProductImages", "ProductID");
            AddForeignKey("dbo.ProductImages", "ProductID", "dbo.Products", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductImages", "ProductID", "dbo.Products");
            DropIndex("dbo.ProductImages", new[] { "ProductID" });
            AlterColumn("dbo.ProductImages", "ProductID", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductImages", "ProductID");
            AddForeignKey("dbo.ProductImages", "ProductID", "dbo.Products", "ID", cascadeDelete: true);
        }
    }
}
