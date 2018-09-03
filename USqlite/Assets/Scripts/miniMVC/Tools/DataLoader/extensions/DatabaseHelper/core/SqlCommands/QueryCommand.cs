


using Mono.Data.Sqlite;

namespace USqlite
{
    public class QueryCommand : BaseCommand
    {
        public QueryCommand(SqliteConnection connection) : base(connection)
        {
            commandText = "SELECT * FROM ";
        }
    }
}