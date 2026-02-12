namespace EmployeesMVC4._7.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmployeeLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.Employees", "Longitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "Longitude");
            DropColumn("dbo.Employees", "Latitude");
        }
    }
}
