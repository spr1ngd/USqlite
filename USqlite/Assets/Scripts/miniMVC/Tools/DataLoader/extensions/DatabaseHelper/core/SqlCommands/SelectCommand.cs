
using Mono.Data.Sqlite;

namespace USqlite
{
    public class SelectCommand : BaseCommand
    {
        public SelectCommand(SqliteConnection connection,string tableName) : base(connection,tableName)
        {
            m_commandText = string.Format("SELECT * FROM {0}",tableName);
        }
    }
}