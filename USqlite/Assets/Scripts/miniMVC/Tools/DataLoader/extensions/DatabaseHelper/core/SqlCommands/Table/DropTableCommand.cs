
using System;
using Mono.Data.Sqlite;

namespace USqlite
{
    public class DropTableCommand : BaseCommand
    {
        public DropTableCommand(SqliteConnection connection , Type type , string tableName) : base(connection,type,tableName)
        { 
            m_commandText = string.Format("DROP TABLE {0}",this.tableName);
        }
    }
}