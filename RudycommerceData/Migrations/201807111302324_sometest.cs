namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sometest : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SpecificationEnums", "Specification_ID", "dbo.Specifications");
            DropIndex("dbo.SpecificationEnums", new[] { "Specification_ID" });
            RenameColumn(table: "dbo.SpecificationEnums", name: "Specification_ID", newName: "SpecificationID");
            RenameColumn(table: "dbo.Values_ProductSpecifications", name: "EnumerationValue_ID", newName: "SpecificationEnum_ID");
            RenameIndex(table: "dbo.Values_ProductSpecifications", name: "IX_EnumerationValue_ID", newName: "IX_SpecificationEnum_ID");
            AlterColumn("dbo.SpecificationEnums", "SpecificationID", c => c.Int(nullable: false));
            CreateIndex("dbo.SpecificationEnums", "SpecificationID");
            AddForeignKey("dbo.SpecificationEnums", "SpecificationID", "dbo.Specifications", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SpecificationEnums", "SpecificationID", "dbo.Specifications");
            DropIndex("dbo.SpecificationEnums", new[] { "SpecificationID" });
            AlterColumn("dbo.SpecificationEnums", "SpecificationID", c => c.Int());
            RenameIndex(table: "dbo.Values_ProductSpecifications", name: "IX_SpecificationEnum_ID", newName: "IX_EnumerationValue_ID");
            RenameColumn(table: "dbo.Values_ProductSpecifications", name: "SpecificationEnum_ID", newName: "EnumerationValue_ID");
            RenameColumn(table: "dbo.SpecificationEnums", name: "SpecificationID", newName: "Specification_ID");
            CreateIndex("dbo.SpecificationEnums", "Specification_ID");
            AddForeignKey("dbo.SpecificationEnums", "Specification_ID", "dbo.Specifications", "ID");
        }
    }
}
