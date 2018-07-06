namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class brand : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LogoURL = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        ModifiedAt = c.DateTime(nullable: false),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Products", "Brand_ID", c => c.Int());
            CreateIndex("dbo.Products", "Brand_ID");
            AddForeignKey("dbo.Products", "Brand_ID", "dbo.Brands", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "Brand_ID", "dbo.Brands");
            DropIndex("dbo.Products", new[] { "Brand_ID" });
            DropColumn("dbo.Products", "Brand_ID");
            DropTable("dbo.Brands");
        }
    }
}
