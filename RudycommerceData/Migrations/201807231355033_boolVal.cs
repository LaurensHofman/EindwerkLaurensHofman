namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class boolVal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Value_ProductSpecification", "BoolValue", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Value_ProductSpecification", "BoolValue");
        }
    }
}
