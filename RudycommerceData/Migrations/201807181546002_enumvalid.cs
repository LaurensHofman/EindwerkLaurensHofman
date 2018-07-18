namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class enumvalid : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Value_ProductSpecification", "EnumValueID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Value_ProductSpecification", "EnumValueID", c => c.Int());
        }
    }
}
