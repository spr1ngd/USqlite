
using System;
using System.Collections.Generic;

namespace miniMVC.USqlite
{
    /// <summary>
    /// 自定义序列化/反序列化对象
    /// </summary>
    public class SqliteCustomSerializeFunc
    {
        public delegate string CustomSerializeFunc(object data);
        public delegate object CustomDeserializeFunc(string databaseValue);

        private readonly IDictionary<Type,CustomSerializeFunc> serializeDic = null;
        private readonly IDictionary<Type,CustomDeserializeFunc> deserializeDic = null;

        public SqliteCustomSerializeFunc()
        {
            serializeDic = new Dictionary<Type,CustomSerializeFunc>();
            deserializeDic = new Dictionary<Type,CustomDeserializeFunc>();
        }

        public void RegisterCustomSerializeFunc(Type type,CustomSerializeFunc serialize)
        {
            if(null != serialize && !serializeDic.ContainsKey(type))
                serializeDic.Add(type,serialize);
        }

        public void RegisterCustomDeserializeFunc(Type type,CustomDeserializeFunc deserialize)
        {
            if(null != deserialize && !deserializeDic.ContainsKey(type))
                deserializeDic.Add(type,deserialize);
        }

        public void RegisterCustomSerializeFunc(Type type,CustomSerializeFunc serialize = null,CustomDeserializeFunc deserialize = null)
        {
            if( null != serialize && !serializeDic.ContainsKey(type))
                serializeDic.Add(type,serialize);
            if( null != deserialize && !deserializeDic.ContainsKey(type))
                deserializeDic.Add(type,deserialize);
        }

        public bool ContainSerializeFunc( Type type )
        {
            return serializeDic.ContainsKey(type);
        }

        public bool ContainDeserializeFunc( Type type )
        {
            return deserializeDic.ContainsKey(type);
        }

        public bool TryGetSerializaFunc( Type type ,out CustomSerializeFunc serializeFunc )
        {
            return serializeDic.TryGetValue(type, out serializeFunc);
        }

        public bool TryGetDeserializeFunc( Type type, out CustomDeserializeFunc deserializeFunc )
        {
            return deserializeDic.TryGetValue(type, out deserializeFunc);
        }
    }
}