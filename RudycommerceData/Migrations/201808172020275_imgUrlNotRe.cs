namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class imgUrlNotRe : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductImages", "ImageURL", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductImages", "ImageURL", c => c.String(nullable: false));
        }
    }
}
