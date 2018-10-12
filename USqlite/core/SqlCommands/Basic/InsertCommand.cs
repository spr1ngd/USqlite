
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Mono.Data.Sqlite;

namespace USqlite
{
    public class InsertCommand<T> : BaseCommand
    {
        public InsertCommand(SqliteConnection connection,Type type,T obj,string tableName) : base(connection,type,tableName)
        {
            string[] columeNames = null;
            string[] columeValues = null;

            GetColumnsValue(obj,out columeNames,out columeValues);
            if(columeNames.Length <= 0 || columeNames.Length != columeValues.Length)
            {
                throw new USqliteException("插入数据异常");
            }
            string columes = columeNames[0];
            for(int i = 1; i < columeNames.Length; i++)
                columes += "," + columeNames[i];

            string values = columeValues[0];
            for(int i = 1; i < columeNames.Length; i++)
                values += "," + columeValues[i];
            m_commandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",this.tableName,columes,values);
        }

        protected string GetColumnsValue(object obj, out string[] columeNames, out string[] columeValues)
        {
            Type type = obj.GetType();
            FieldInfo[] fieldInfos = type.GetFields();
            IList<CellMapper> cells = new List<CellMapper>();
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                var columeAtt = fieldInfo.AttributeOf<ColumnAttribute>();
                if (null != columeAtt)
                {
                    CellMapper cell = new CellMapper();
                    cell.columeName = columeAtt.columnName;
                    if (string.IsNullOrEmpty(columeAtt.columnName))
                        cell.columeName = fieldInfo.Name;
                    cell.dbType = columeAtt.columnType;
                    cell.type = fieldInfo.FieldType;
                    cell.notNull = columeAtt.columeNotNull;
                    var primaryKeyAtt = fieldInfo.AttributeOf<PrimaryKeyAttribute>();
                    cell.isPrimaryKey = null != primaryKeyAtt;
                    if (columeAtt.columnType == (DbType.Int32 | DbType.Int64 | DbType.Int16))
                    {
                        var autoIncrease = fieldInfo.AttributeOf<AutoIncrementAttribute>();
                        cell.isAutoIncrease = null != autoIncrease;
                    }
                    cell.value = fieldInfo.GetValue(obj);
                    cells.Add(cell);
                }
            }

            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                var columeAtt = propertyInfo.AttributeOf<ColumnAttribute>();
                if (null != columeAtt)
                {
                    CellMapper cell = new CellMapper();
                    cell.columeName = columeAtt.columnName;
                    if (string.IsNullOrEmpty(columeAtt.columnName))
                        cell.columeName = propertyInfo.Name;
                    cell.dbType = columeAtt.columnType;
                    cell.type = propertyInfo.PropertyType;
                    cell.notNull = columeAtt.columeNotNull;
                    var primaryKeyAtt = propertyInfo.AttributeOf<PrimaryKeyAttribute>();
                    cell.isPrimaryKey = null != primaryKeyAtt;
                    if (columeAtt.columnType == (DbType.Int32 | DbType.Int64 | DbType.Int16))
                    {
                        var autoIncrease = propertyInfo.AttributeOf<AutoIncrementAttribute>();
                        cell.isAutoIncrease = null != autoIncrease;
                    }
                    cell.value = propertyInfo.GetValue(obj, null);
                    cells.Add(cell);
                }
            }

            columeNames = new string[cells.Count];
            columeValues = new string[cells.Count];
            for (int i = 0; i < columeNames.Length; i++)
            {
                columeNames[i] = cells[i].columeName;
                columeValues[i] = Orm.GetWriteFunc(cells[i].type)(cells[i].value).ToString();
            }

            TableAttribute tableAtt = type.AttributeOf<TableAttribute>();
            if (null != tableAtt)
                return tableAtt.tableName;
            return type.Name;

        }
    }
}