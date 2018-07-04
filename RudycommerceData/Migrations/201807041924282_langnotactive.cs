namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class langnotactive : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Languages", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Languages", "IsActive", c => c.Boolean(nullable: false));
        }
    }
}
