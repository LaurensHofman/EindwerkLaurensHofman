namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class imgUrlNotRe1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.ProductImages", new[] { "ProductID" });
            AlterColumn("dbo.ProductImages", "ProductID", c => c.Int());
            CreateIndex("dbo.ProductImages", "ProductID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ProductImages", new[] { "ProductID" });
            AlterColumn("dbo.ProductImages", "ProductID", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductImages", "ProductID");
        }
    }
}
