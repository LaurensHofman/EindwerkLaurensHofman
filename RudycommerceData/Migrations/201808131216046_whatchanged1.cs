namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class whatchanged1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.IncomingOrders", "PaymentComplete", c => c.Boolean(nullable: false));
            DropColumn("dbo.IncomingOrders", "CardNumber");
            DropColumn("dbo.IncomingOrders", "ExpirationDate");
            DropColumn("dbo.IncomingOrders", "NameOnCard");
        }
        
        public override void Down()
        {
            AddColumn("dbo.IncomingOrders", "NameOnCard", c => c.String());
            AddColumn("dbo.IncomingOrders", "ExpirationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.IncomingOrders", "CardNumber", c => c.String());
            DropColumn("dbo.IncomingOrders", "PaymentComplete");
        }
    }
}
