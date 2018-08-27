
using System;

namespace miniMVC.USqlite
{
    /// <summary>
    /// Sqlite 序列化特性 , 标记特性的属性可以使得改字段或属性成为主键
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field,AllowMultiple = true,Inherited = true)]
    public class USqlitePrimaryKeyAttribute : Attribute
    {
        public USqlitePrimaryKeyAttribute(){}
    }
}