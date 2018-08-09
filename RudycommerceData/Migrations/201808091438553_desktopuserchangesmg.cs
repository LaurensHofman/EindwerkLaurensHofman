namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class desktopuserchangesmg : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DesktopUsers", "LastName", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.DesktopUsers", "FirstName", c => c.String(nullable: false, maxLength: 30));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DesktopUsers", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.DesktopUsers", "LastName", c => c.String(nullable: false));
        }
    }
}
