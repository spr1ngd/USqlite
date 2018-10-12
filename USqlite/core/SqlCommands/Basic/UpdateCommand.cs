
using System;
using System.Linq.Expressions;
using Mono.Data.Sqlite;

namespace USqlite
{
    public class UpdateCommand<T> : BaseCommand
    {
        public UpdateCommand(SqliteConnection connection,Type type,string tableName) : base(connection,type,tableName)
        {
            m_commandText = string.Format("UPDATE {0}",this.tableName);
        }

        public void Set(Expression<Func<T,string[]>> setColumn,object[] values)
        {
            string columeNames = setColumn.Body.ToString();
            int startIndex = columeNames.IndexOf("{",StringComparison.Ordinal) + 1;
            int endIndex = columeNames.IndexOf("}",StringComparison.Ordinal);
            columeNames = columeNames.Substring(startIndex,endIndex-startIndex);
            columeNames = columeNames.Replace("&&","AND").Replace("(","").Replace(")","");
            int pointIndex = columeNames.IndexOf(".", StringComparison.Ordinal)+1;
            string proprotyName = columeNames.Substring(0,pointIndex);
            columeNames = columeNames.Replace(proprotyName, "");
            string[] properties = columeNames.Split(',');
            string sql = string.Empty;
            for (int index = 0; index < properties.Length; index++)
            {
                var writeFunc = Orm.GetWriteFunc(values[index].GetType());
                sql += properties[index] + "=" + writeFunc(values[index]);
                if (index < properties.Length-1)
                    sql += ",";
            }
            m_commandText += string.Format(" SET {0}",sql);
        }
    }
}