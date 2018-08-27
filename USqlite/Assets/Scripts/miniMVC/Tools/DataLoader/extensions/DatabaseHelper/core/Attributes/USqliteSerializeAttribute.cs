
using System;

namespace miniMVC.USqlite
{
    /// <summary>
    /// Sqlite 序列化特性 , 标记特性的属性可以参与Sqlite存取的序列化与反序化
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field,AllowMultiple = false,Inherited = true)]
    public class USqliteSerializeAttribute : Attribute
    {
        public USqliteSerializeAttribute() { }
    }
}