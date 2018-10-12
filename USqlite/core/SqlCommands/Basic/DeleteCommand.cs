
using System;
using Mono.Data.Sqlite;

namespace USqlite
{
    public class DeleteCommand : BaseCommand
    {
        public DeleteCommand(SqliteConnection connection,Type type,string tableName) : base(connection,type,tableName)
        {
            m_commandText = string.Format("DELETE FROM {0}",this.tableName);
        }
    }
}