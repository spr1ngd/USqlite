
using System;

namespace USqlite
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field,AllowMultiple = false,Inherited = false)]
    public class ColumnAttribute : Attribute
    {
        public string columnName { get; set; }

        public ColumnAttribute()
        {

        }

        public ColumnAttribute( string columnName )
        {
            this.columnName = columnName;
        }
    }
}