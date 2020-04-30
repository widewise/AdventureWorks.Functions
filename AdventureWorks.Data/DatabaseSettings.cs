namespace AdventureWorks.Data
{
    public class DatabaseSettings
    {
        public string DbConnectionStringName { get; }

        public DatabaseSettings(
            string dbConnectionStringName)
        {
            DbConnectionStringName = dbConnectionStringName;
        }
    }
}