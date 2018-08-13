namespace RudycommerceData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class clients : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AccountUser = c.Boolean(nullable: false),
                        StreetAndNumber = c.String(nullable: false, maxLength: 80),
                        MailBox = c.String(),
                        City = c.String(nullable: false, maxLength: 60),
                        PostalCode = c.String(nullable: false, maxLength: 10),
                        CountryCode = c.String(nullable: false),
                        WantsNewsletter = c.Boolean(nullable: false),
                        EncryptedPassword = c.String(),
                        Salt = c.String(),
                        LastName = c.String(nullable: false, maxLength: 30),
                        FirstName = c.String(nullable: false, maxLength: 30),
                        Email = c.String(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        ModifiedAt = c.DateTime(nullable: false),
                        DeletedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Clients");
        }
    }
}
