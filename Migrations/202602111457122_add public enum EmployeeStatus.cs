namespace EmployeesMVC4._7.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addpublicenumEmployeeStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "Status");
        }
    }
}
