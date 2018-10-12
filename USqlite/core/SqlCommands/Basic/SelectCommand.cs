
using System;
using Mono.Data.Sqlite;

namespace USqlite
{
    public class SelectCommand : BaseCommand
    {
        public SelectCommand(SqliteConnection connection,Type type,string tableName) : base(connection,type,tableName)
        {
            m_commandText = string.Format("SELECT * FROM {0}",this.tableName);
        }
    }
}