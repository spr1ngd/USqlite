
using System;

namespace USqlite
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false,Inherited = false)]
    public class TableAttribute : Attribute
    {
        public string tableName { get; set; }

        public TableAttribute()
        {

        }

        public TableAttribute( string tableName )
        {
            this.tableName = tableName;
        }
    }
}