namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minStock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "MinimumStock", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "MinimumStock");
        }
    }
}
