
using Mono.Data.Sqlite;

namespace USqlite
{
    public class InsertCommand : BaseCommand
    {
        public InsertCommand(SqliteConnection connection,string tableName) : base(connection,tableName)
        {
            m_commandText = string.Format("INSERT INTO {0}",tableName);
        }
    }
}