namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedSellingWell : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Products", "SellingWell");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "SellingWell", c => c.Boolean(nullable: false));
        }
    }
}
