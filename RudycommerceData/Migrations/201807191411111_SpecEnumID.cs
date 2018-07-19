namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SpecEnumID : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Value_ProductSpecification", name: "SpecificationEnum_ID", newName: "SpecificationEnumID");
            RenameIndex(table: "dbo.Value_ProductSpecification", name: "IX_SpecificationEnum_ID", newName: "IX_SpecificationEnumID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Value_ProductSpecification", name: "IX_SpecificationEnumID", newName: "IX_SpecificationEnum_ID");
            RenameColumn(table: "dbo.Value_ProductSpecification", name: "SpecificationEnumID", newName: "SpecificationEnum_ID");
        }
    }
}
