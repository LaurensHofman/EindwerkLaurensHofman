namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class langiso2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Languages", new[] { "ISO" });
            AlterColumn("dbo.Languages", "ISO", c => c.String(nullable: false, maxLength: 2));
            CreateIndex("dbo.Languages", "ISO", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Languages", new[] { "ISO" });
            AlterColumn("dbo.Languages", "ISO", c => c.String(nullable: false, maxLength: 3));
            CreateIndex("dbo.Languages", "ISO", unique: true);
        }
    }
}
