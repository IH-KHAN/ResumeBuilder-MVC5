namespace ResumeBuilder_1291763.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MKDO : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Experiences", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Experiences", "EndDate", c => c.DateTime(nullable: false));
        }
    }
}
