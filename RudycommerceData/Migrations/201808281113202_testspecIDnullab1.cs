namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testspecIDnullab1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SpecificationEnums", "SpecificationID", "dbo.Specifications");
            DropIndex("dbo.SpecificationEnums", new[] { "SpecificationID" });
            AlterColumn("dbo.SpecificationEnums", "SpecificationID", c => c.Int());
            CreateIndex("dbo.SpecificationEnums", "SpecificationID");
            AddForeignKey("dbo.SpecificationEnums", "SpecificationID", "dbo.Specifications", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SpecificationEnums", "SpecificationID", "dbo.Specifications");
            DropIndex("dbo.SpecificationEnums", new[] { "SpecificationID" });
            AlterColumn("dbo.SpecificationEnums", "SpecificationID", c => c.Int(nullable: false));
            CreateIndex("dbo.SpecificationEnums", "SpecificationID");
            AddForeignKey("dbo.SpecificationEnums", "SpecificationID", "dbo.Specifications", "ID", cascadeDelete: true);
        }
    }
}
