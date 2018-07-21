namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usernameUnique : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DesktopUsers", "Username", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("dbo.DesktopUsers", "Username", unique: true, name: "IX_UniqueUsername");
        }
        
        public override void Down()
        {
            DropIndex("dbo.DesktopUsers", "IX_UniqueUsername");
            AlterColumn("dbo.DesktopUsers", "Username", c => c.String(nullable: false));
        }
    }
}
