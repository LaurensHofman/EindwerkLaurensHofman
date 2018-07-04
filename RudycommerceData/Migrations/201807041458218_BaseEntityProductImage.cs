namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BaseEntityProductImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductImages", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.ProductImages", "ModifiedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.ProductImages", "DeletedAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductImages", "DeletedAt");
            DropColumn("dbo.ProductImages", "ModifiedAt");
            DropColumn("dbo.ProductImages", "CreatedAt");
        }
    }
}
