
using Mono.Data.Sqlite;

namespace USqlite
{
    public class DeleteCommand : BaseCommand
    {
        public DeleteCommand(SqliteConnection connection,string tableName) : base(connection,tableName)
        {
            m_commandText = string.Format("DELETE FROM {0}",tableName);
        }
    }
}