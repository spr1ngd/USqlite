
using System;
using System.Data;

namespace USqlite
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field,AllowMultiple = false,Inherited = false)]
    public class ColumnAttribute : Attribute
    {
        public string columnName { get; set; }
        public DbType columnType { get; set; }
        public bool columeNotNull { get; set; }

        public ColumnAttribute()
        {
            columnType = DbType.String;
            columeNotNull = true;
        }

        public ColumnAttribute( string columnName )
        {
            this.columnName = columnName;
        }

        public ColumnAttribute(string columeName,DbType columeType = DbType.String,bool isNotNull = true)
        {
            this.columnName = columeName;
            this.columnType = columeType;
            this.columeNotNull = isNotNull;
        }
    }
}