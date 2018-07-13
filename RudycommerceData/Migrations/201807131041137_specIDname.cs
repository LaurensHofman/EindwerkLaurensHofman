namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class specIDname : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.LocalizedSpecifications", name: "PropertyID", newName: "SpecificationID");
            RenameIndex(table: "dbo.LocalizedSpecifications", name: "IX_PropertyID", newName: "IX_SpecificationID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.LocalizedSpecifications", name: "IX_SpecificationID", newName: "IX_PropertyID");
            RenameColumn(table: "dbo.LocalizedSpecifications", name: "SpecificationID", newName: "PropertyID");
        }
    }
}
