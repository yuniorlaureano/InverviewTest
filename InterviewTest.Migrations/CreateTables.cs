using FluentMigrator;

namespace InterviewTest.Migrations
{
    [Migration(1234554)]
    public class CreateTables : Migration
    {
        public override void Down()
        {
            var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "RemoveTables.sql");
            var script = File.ReadAllText(scriptPath);
            Execute.Sql(script);
        }

        public override void Up()
        {
            var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "CreateTables.sql");
            var script = File.ReadAllText(scriptPath);
            Execute.Sql(script);
        }
    }
}
