
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Data.Sqlite;

namespace USqlite
{ 
    public class BaseCommand
    {
        private readonly SqliteConnection m_sqliteConnection = null;
        protected string m_commandText = string.Empty;
        public string commandText { get { return string.Format("{0} {1} {2}",m_commandText,conditionCommand,orderbyCommand); } }

        private readonly string m_tableName;
        protected string tableName
        {
            get
            {
                if(!string.IsNullOrEmpty(m_tableName))
                    return m_tableName;
                string result = "";
                if(string.IsNullOrEmpty(result))
                    result = mapperType.AttributeOf<TableAttribute>().tableName;
                if(string.IsNullOrEmpty(result))
                    result = mapperType.Name;
                return result;
            }
        }

        protected Type mapperType = null;
        protected string conditionCommand = string.Empty;
        protected string orderbyCommand = string.Empty;

        public BaseCommand(SqliteConnection connection,Type type,string tableName)
        {
            this.m_sqliteConnection = connection;
            this.mapperType = type;
            this.m_tableName = tableName;
        }

        public virtual SqliteDataReader Execute()
        {
            try
            {
                var command = m_sqliteConnection.CreateCommand();
                command.CommandText = commandText;
                UnityEngine.Debug.Log(string.Format("<color=green>USqlite execute : {0}</color>",command.CommandText));
                return command.ExecuteReader();
            }
            catch(Exception @exception)
            {
                throw new USqliteException("Database Rollback",@exception.InnerException);
            }
        }

        public virtual BaseCommand Where(string condition)
        {
            if (!string.IsNullOrEmpty(condition))
            {
                if (string.IsNullOrEmpty(conditionCommand)) conditionCommand = string.Format("WHERE {0}",condition);
                else conditionCommand += string.Format("AND {0}",condition);
            }
            return this;
        }

        public virtual BaseCommand Where<T>( Expression<Func<T,bool>> whereExpression )
        {
            var condition = whereExpression.Body.ToString();
            if(condition.Contains("("))
            {
                int tagStart = condition.IndexOf("(",StringComparison.Ordinal) + 1;
                int tagEnd = condition.IndexOf(".",StringComparison.Ordinal);
                string keyPoint = condition.Substring(tagStart,tagEnd);
                condition = condition.Replace(keyPoint,"");
            }
            else
            {
                int tagEnd = condition.IndexOf(".",StringComparison.Ordinal);
                string keyPoint = condition.Substring(0,tagEnd);
                condition = condition.Replace(keyPoint,"");
            }
            condition = condition.Replace("&&", "AND").Replace("(","").Replace(")","");
            return Where(condition);
        }

        public virtual BaseCommand OrderBy(string columnName)
        {
            if(!string.IsNullOrEmpty(columnName))
            {
                if(string.IsNullOrEmpty(orderbyCommand)) orderbyCommand = string.Format("ORDER BY {0}",columnName);
                else orderbyCommand += string.Format(", {0}",columnName);
            }
            return this;
        }

        public virtual BaseCommand OrderBy<T>( Expression<Func<T,bool>> orderByExpression )
        {
            return this;
        }

        protected string GetColumes(Type type,out string[] columeNames,out string[] columeTypes)
        {
            FieldInfo[] fieldInfos = type.GetFields();
            IList<CellMapper> cells = new List<CellMapper>();
            foreach(FieldInfo fieldInfo in fieldInfos)
            {
                var columeAtt = fieldInfo.AttributeOf<ColumnAttribute>();
                if(null != columeAtt)
                {
                    CellMapper cell = new CellMapper();
                    cell.columeName = columeAtt.columnName;
                    if(string.IsNullOrEmpty(columeAtt.columnName))
                        cell.columeName = fieldInfo.Name;
                    cell.dbType = columeAtt.columnType;
                    cell.notNull = columeAtt.columeNotNull;
                    var primaryKeyAtt = fieldInfo.AttributeOf<PrimaryKeyAttribute>();
                    cell.isPrimaryKey = null != primaryKeyAtt;
                    if(columeAtt.columnType == (DbType.Int32 | DbType.Int64 | DbType.Int16))
                    {
                        var autoIncrease = fieldInfo.AttributeOf<AutoIncrementAttribute>();
                        cell.isAutoIncrease = null != autoIncrease;
                    }
                    cells.Add(cell);
                }
            }

            PropertyInfo[] properties = type.GetProperties();
            foreach(PropertyInfo propertyInfo in properties)
            {
                var columeAtt = propertyInfo.AttributeOf<ColumnAttribute>();
                if(null != columeAtt)
                {
                    CellMapper cell = new CellMapper();
                    cell.columeName = columeAtt.columnName;
                    if(string.IsNullOrEmpty(columeAtt.columnName))
                        cell.columeName = propertyInfo.Name;
                    cell.dbType = columeAtt.columnType;
                    cell.notNull = columeAtt.columeNotNull;
                    var primaryKeyAtt = propertyInfo.AttributeOf<PrimaryKeyAttribute>();
                    cell.isPrimaryKey = null != primaryKeyAtt;
                    if(columeAtt.columnType == DbType.Int32 || columeAtt.columnType == DbType.Int64 | columeAtt.columnType == DbType.Int16)
                    {
                        var autoIncrease = propertyInfo.AttributeOf<AutoIncrementAttribute>();
                        cell.isAutoIncrease = null != autoIncrease;
                    }
                    cells.Add(cell);
                }
            }

            columeNames = new string[cells.Count];
            columeTypes = new string[cells.Count];
            for(int i = 0; i < columeNames.Length; i++)
            {
                columeNames[i] = cells[i].columeName;
                columeTypes[i] = cells[i].dbType.ToString();
                if(cells[i].isPrimaryKey)
                    columeTypes[i] += " PRIMARY KEY";
                if(cells[i].isAutoIncrease)
                    columeTypes[i] += "AUTOINCREMENT";
            }

            TableAttribute tableAtt = type.AttributeOf<TableAttribute>();
            if(null != tableAtt)
                return tableAtt.tableName;
            return type.Name;
        }
    }
}