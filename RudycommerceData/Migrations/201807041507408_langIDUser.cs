namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class langIDUser : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.DesktopUsers", name: "PreferredLanguage_ID", newName: "PreferredLanguageID");
            RenameIndex(table: "dbo.DesktopUsers", name: "IX_PreferredLanguage_ID", newName: "IX_PreferredLanguageID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.DesktopUsers", name: "IX_PreferredLanguageID", newName: "IX_PreferredLanguage_ID");
            RenameColumn(table: "dbo.DesktopUsers", name: "PreferredLanguageID", newName: "PreferredLanguage_ID");
        }
    }
}
