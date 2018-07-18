namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nameVals : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Values_ProductSpecifications", newName: "Value_ProductSpecification");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Value_ProductSpecification", newName: "Values_ProductSpecifications");
        }
    }
}
