namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class whatchanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IncomingOrders", "TotalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.IncomingOrders", "TotalPrice");
        }
    }
}
