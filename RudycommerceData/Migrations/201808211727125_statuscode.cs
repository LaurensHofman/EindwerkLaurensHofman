namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class statuscode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IncomingOrders", "StatusCode", c => c.Int(nullable: false));
            DropColumn("dbo.IncomingOrders", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IncomingOrders", "Status", c => c.String());
            DropColumn("dbo.IncomingOrders", "StatusCode");
        }
    }
}
