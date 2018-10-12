
using System;

namespace USqlite
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field,AllowMultiple = false,Inherited = false)]
    public class NotNullAttribute : Attribute
    {
        public NotNullAttribute(){}
    }
}