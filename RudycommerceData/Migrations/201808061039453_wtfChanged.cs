namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wtfChanged : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DesktopUsers", "EncryptedPassword", c => c.String());
            AlterColumn("dbo.DesktopUsers", "Salt", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DesktopUsers", "Salt", c => c.String(nullable: false));
            AlterColumn("dbo.DesktopUsers", "EncryptedPassword", c => c.String(nullable: false));
        }
    }
}
