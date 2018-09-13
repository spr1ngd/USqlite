
using System;
using Mono.Data.Sqlite;

namespace USqlite
{
    public class CreateTableCommand : BaseCommand
    {
        public CreateTableCommand(SqliteConnection connection,Type type,string tableName) : base(connection,type,tableName)
        {
            string[] columeNames = null;
            string[] columeTypes = null;
            GetColumes(type,out columeNames,out columeTypes);
            string parameters = string.Empty;
            for(int columeId = 0; columeId < columeNames.Length; columeId++)
            {
                parameters += string.Format("{0} {1}",columeNames[columeId],columeTypes[columeId]);
                if(columeId < columeNames.Length - 1)
                    parameters += ",";
            }
            m_commandText = string.Format("CREATE TABLE {0} {1}",this.tableName,string.IsNullOrEmpty(parameters) ? "" : string.Format("({0})",parameters));
        }
    }
}