
using Mono.Data.Sqlite;

namespace USqlite
{
    public class UpdateCommand : BaseCommand
    {
        public UpdateCommand(SqliteConnection connection,string tableName) : base(connection,tableName)
        {
            m_commandText = string.Format("UPDATE {0}",tableName);
        }
    }
}