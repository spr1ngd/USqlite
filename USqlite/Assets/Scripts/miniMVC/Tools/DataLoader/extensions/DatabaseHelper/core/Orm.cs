
using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Data.Sqlite;

namespace USqlite
{
    public static class Orm
    {
        private static ColumnMapper m_columnMapper = new ColumnMapper();
        private static readonly InstanceFactory m_instanceFactory = new InstanceFactory();
        private static readonly IDictionary<Type,ColumnMapper> m_tableMapperDic = new Dictionary<Type,ColumnMapper>();
        private static readonly SqliteCustomSerializeFunc m_customSerializeFun = new SqliteCustomSerializeFunc();

        private static readonly Dictionary<Type,Func<SqliteDataReader,int,object>> m_readFuncsDic = new Dictionary<Type,Func<SqliteDataReader,int,object>>();
        private static readonly Dictionary<Type,Func<object,object>> m_writeFuncDic = new Dictionary<Type, Func<object, object>>();

        public static List<T> Mapping2List<T>( SqliteDataReader dataReader )
        {
            return new TableMapper<T>(dataReader,m_instanceFactory.ConstructeInstance(typeof(T))).ToObject();
        }

        public static bool TryGetMember(string memberName,out MemberInfo memberInfo)
        {
            return m_columnMapper.TryGetMember(memberName,out memberInfo);
        }

        public static void MappingType(Type type)
        {
            ColumnMapper result = null;
            if(!m_tableMapperDic.TryGetValue(type,out result))
            {
                result = new ColumnMapper();
                var fieldInfos = type.GetFields();
                foreach(var fieldInfo in fieldInfos)
                {
                    var att = fieldInfo.GetCustomAttributes(typeof(ColumnAttribute),false);
                    if(att.Length > 0)
                        result.AddMemberInfo(fieldInfo.Name.ToUpper(),fieldInfo);
                }
                var propertyInfos = type.GetProperties();
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    var att = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute),false);
                    if(att.Length > 0)
                        result.AddMemberInfo(propertyInfo.Name.ToUpper(),propertyInfo);
                }
                m_tableMapperDic.Add(type,result);
            }
            m_columnMapper = result;
        }

        public static Func<SqliteDataReader,int,object> GetReadFunc(Type propertyFileType)
        {
            Func<SqliteDataReader,int,object> func = null;
            if(!m_readFuncsDic.TryGetValue(propertyFileType,out func))
            {
                if(propertyFileType == typeof(Int32))
                    func = (reader,columnId) => reader.GetInt32(columnId);
                else if(propertyFileType == typeof(Int64))
                    func = (reader,columnId) => reader.GetInt64(columnId);
                else if(propertyFileType == typeof(string))
                    func = (reader,columnId) => reader.GetString(columnId);
                else if(propertyFileType == typeof(bool))
                    func = (reader,columnId) => reader.GetBoolean(columnId);
                else if(propertyFileType == typeof(float))
                    func = (reader,columnId) => reader.GetFloat(columnId);
                else
                {
                    SqliteCustomSerializeFunc.CustomDeserializeFunc deserializeFunc = null;
                    m_customSerializeFun.TryGetDeserializeFunc(propertyFileType, out deserializeFunc);
                    if (null == deserializeFunc)
                        throw new USqliteException("尚未注册此类型的反序列化方法");
                    func = (reader, columnId) =>
                    {
                        var value = reader.GetString(columnId);
                        return deserializeFunc(value);
                    };
                }
                m_readFuncsDic.Add(propertyFileType,func);
            }
            return func;
        }

        public static Func<object,object> GetWriteFunc(Type propertyFieldType)
        {
            Func<object,object> func = null;
            if (!m_writeFuncDic.TryGetValue(propertyFieldType, out func))
            {
                if(propertyFieldType == typeof(Enum))
                {
                    func = (value) => string.Format(@"'{0}'",value.ToString());
                }
                else if(propertyFieldType == typeof(Int32) || propertyFieldType == typeof(Int64))
                {
                    func = (value) => value;
                }
                else if(propertyFieldType == typeof(float))
                {
                    func = (value) => value;
                }
                else if(propertyFieldType == typeof(double))
                {
                    func = (value) => value;
                }
                else if(propertyFieldType == typeof(string))
                {
                    func = (value) => string.Format(@"'{0}'",null==value?@"":value.ToString());
                }
                else if(propertyFieldType == typeof(bool))
                {
                    func = (value) => value.ToString();
                }
                else
                {
                    SqliteCustomSerializeFunc.CustomSerializeFunc serializeFunc = null;
                    m_customSerializeFun.TryGetSerializaFunc(propertyFieldType,out serializeFunc);
                    if(null == serializeFunc)
                        throw new USqliteException(string.Format("尚未注册此 [{0}] 类型的序列化方法",propertyFieldType));
                    func = (value) => serializeFunc(value);
                }
                m_writeFuncDic.Add(propertyFieldType,func);
            }
            return func;
        }

        public static void RegisterSerializeFunc(Type type,SqliteCustomSerializeFunc.CustomSerializeFunc serialize)
        {
            m_customSerializeFun.RegisterCustomSerializeFunc(type,serialize);
        }

        public static void RegisterDeserializeFunc(Type type,SqliteCustomSerializeFunc.CustomDeserializeFunc deserialize)
        {
            m_customSerializeFun.RegisterCustomDeserializeFunc(type,deserialize);
        }

        public static void RegisterCustomSerializeFunc(Type type,SqliteCustomSerializeFunc.CustomSerializeFunc serialize,SqliteCustomSerializeFunc.CustomDeserializeFunc deserialize)
        {
            RegisterSerializeFunc(type,serialize);
            RegisterDeserializeFunc(type,deserialize);
        }
    }
}