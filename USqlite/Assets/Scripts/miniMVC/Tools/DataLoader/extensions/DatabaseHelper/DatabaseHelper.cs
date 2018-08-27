
using System;
using miniMVC.USqlite;

namespace miniMVC
{
    public enum E_DatabaseType
    {
        None = -0x01,
        Sqlite = 0x00,
        MySql = 0x01,
    }

    public class DatabaseHelper
    {
        // todo 此处选定一个数据库，然后直接读取Utility接口获取对象或反序列化对象

        public T ToObject<T>(string json) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public T ToObject<T>(string json, Action callback) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public T ToObject<T>(string json, Action<T> callback) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void ToDatabase<T>(T @object) where T : class, new()
        {

        }

        public void ToDatabase<T>(T @object,Action writeComplete) where T : class, new()
        {

        }
    }
}