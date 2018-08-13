namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orders : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IncomingOrderLines",
                c => new
                    {
                        OrderID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        ProductQuantity = c.Int(nullable: false),
                        ProductUnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => new { t.OrderID, t.ProductID })
                .ForeignKey("dbo.Products", t => t.ProductID, cascadeDelete: true)
                .ForeignKey("dbo.IncomingOrders", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.OrderID)
                .Index(t => t.ProductID);
            
            CreateTable(
                "dbo.IncomingOrders",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ClientID = c.Int(nullable: false),
                        Status = c.String(),
                        PaymentOption = c.String(),
                        CardNumber = c.String(),
                        ExpirationDate = c.DateTime(nullable: false),
                        NameOnCard = c.String(),
                        AddrStreetAndNumber = c.String(),
                        AddrMailBox = c.String(),
                        AddrPostalCode = c.String(),
                        AddrCity = c.String(),
                        AddrCountry = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        ModifiedAt = c.DateTime(nullable: false),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Clients", t => t.ClientID, cascadeDelete: true)
                .Index(t => t.ClientID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IncomingOrderLines", "OrderID", "dbo.IncomingOrders");
            DropForeignKey("dbo.IncomingOrders", "ClientID", "dbo.Clients");
            DropForeignKey("dbo.IncomingOrderLines", "ProductID", "dbo.Products");
            DropIndex("dbo.IncomingOrders", new[] { "ClientID" });
            DropIndex("dbo.IncomingOrderLines", new[] { "ProductID" });
            DropIndex("dbo.IncomingOrderLines", new[] { "OrderID" });
            DropTable("dbo.IncomingOrders");
            DropTable("dbo.IncomingOrderLines");
        }
    }
}
