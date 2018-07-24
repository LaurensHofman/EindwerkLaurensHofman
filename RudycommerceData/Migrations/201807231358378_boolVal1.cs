namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class boolVal1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Value_ProductSpecification", "BoolValue", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Value_ProductSpecification", "BoolValue", c => c.Boolean(nullable: false));
        }
    }
}
