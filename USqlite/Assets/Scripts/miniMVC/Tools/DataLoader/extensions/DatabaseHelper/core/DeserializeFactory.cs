
using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

namespace miniMVC.USqlite
{
    public class Property
    {
        public string name;
        public PropertyInfo propertyInfo;
    }

    public class Field
    {
        public string name;
        public FieldInfo fieldInfo;
    }

    public class ClassInfo
    {
        private readonly IList<Property> m_properties = null;
        public IList<Property> properties { get { return m_properties; } }
        private readonly IList<Field> m_fields = null;
        public IList<Field> fields { get { return m_fields; } }

        public ClassInfo()
        {
            m_properties = new List<Property>();
            m_fields = new List<Field>();
        }

        public void AddPropertyInfo(PropertyInfo propertyInfo)
        {
            m_properties.Add(new Property()
            {
                name = propertyInfo.Name.ToUpper(),
                propertyInfo = propertyInfo
            });
        }

        public void AddFieldInfo(FieldInfo fieldInfo)
        {
            m_fields.Add(new Field()
            {
                name = fieldInfo.Name.ToUpper(),
                fieldInfo = fieldInfo
            });
        }

        public bool ContainProperty(PropertyInfo propertyInfo)
        {
            foreach(Property property in m_properties)
            {
                if(property.name.Equals(propertyInfo.Name.ToUpper()))
                    return true;
            }
            return false;
        }

        public bool ContainField(FieldInfo fieldInfo)
        {
            foreach(Field field in m_fields)
            {
                if(field.name.Equals(fieldInfo.Name.ToUpper()))
                    return true;
            }
            return false;
        }
    }

    public delegate object SqliteDeserializeFunc(Type type,object data);

    public class DeserializeFunc
    {
        private readonly IDictionary<Type,SqliteDeserializeFunc> funcs = null;

        public DeserializeFunc()
        {
            funcs = new Dictionary<Type,SqliteDeserializeFunc>();
        }

        public void AddFunc(Type type,SqliteDeserializeFunc func)
        { 
            this.funcs.Add(type,func);
        }

        public bool TryGetFunc(Type type,out SqliteDeserializeFunc func)
        {
            return funcs.TryGetValue(type,out func);
        }
    }

    public class DeserializeFactory
    {
        private readonly SqliteCustomSerializeFunc m_customSerializeFunc = null;
        private readonly IList<SqliteRow> m_dbMetadata = new List<SqliteRow>(); // 数据库存取数据（临时）
        public IList<SqliteRow> dbMetadata { get { return m_dbMetadata; } }

        private readonly IDictionary<Type,ClassInfo> m_classInfoDic = null;
        private readonly DeserializeFunc m_deserializeFunc = null;

        public DeserializeFactory(SqliteCustomSerializeFunc customSerializeFunc)
        {
            m_customSerializeFunc = customSerializeFunc;
            m_deserializeFunc = new DeserializeFunc();
            m_classInfoDic = new Dictionary<Type,ClassInfo>();
            RegisterDeserializeFunc();
        }

        public void Clear()
        {
            dbMetadata.Clear();
        }

        private void RegisterDeserializeFunc() 
        {
            m_deserializeFunc.AddFunc(typeof(int),(TYPE,DATA) => DATA);
            m_deserializeFunc.AddFunc(typeof(float),(TYPE,DATA) => DATA);
            m_deserializeFunc.AddFunc(typeof(double),(TYPE,DATA) => DATA);
            m_deserializeFunc.AddFunc(typeof(string),(TYPE,DATA) => DATA);
            m_deserializeFunc.AddFunc(typeof(bool),(TYPE,DATA) => DATA);
            m_deserializeFunc.AddFunc(typeof(Enum),(TYPE,DATA) => Enum.Parse(TYPE,DATA.ToString()));
        }

        Stopwatch watch = new Stopwatch();

        public object DeserializeObject(Type type,object data)
        {
            object result = null;
            SqliteDeserializeFunc sqliteDeserializeFunc = null;
            m_deserializeFunc.TryGetFunc(type,out sqliteDeserializeFunc);
            if(null != sqliteDeserializeFunc)
            {
                result = sqliteDeserializeFunc(type,data);
                return result;
            }
            if(m_customSerializeFunc.ContainDeserializeFunc(type))
            {
                SqliteCustomSerializeFunc.CustomDeserializeFunc deserializeFunc = null;
                if(m_customSerializeFunc.TryGetDeserializeFunc(type,out deserializeFunc))
                    m_deserializeFunc.AddFunc(type,(TYPE,DATA) => deserializeFunc(DATA.ToString()));
            }
            else if(type.IsArray) // array 
            {
                m_deserializeFunc.AddFunc(type,(TYPE,DATA) =>
                {
                    Type objectType = Deserialize2Array(TYPE);
                    result = Array.CreateInstance(objectType,dbMetadata.Count);
                    Array array = (Array)result;
                    for(int i = 0; i < dbMetadata.Count; i++)
                    {
                        object value = DeserializeObject(objectType,dbMetadata[i]);
                        array.SetValue(value,i);
                    }
                    return result;
                });
            }
            else if(type.IsClass) // object | ilist | idictionary
            {
                m_deserializeFunc.AddFunc(type,(TYPE,DATA) =>
                {
                    if(null != TYPE.GetInterface(typeof(IList).ToString()))
                    {
                        result = Activator.CreateInstance(TYPE);
                        var objectType = Deserialize2IList(TYPE); // IList中的对象类型
                        foreach(SqliteRow databaseMetaData in dbMetadata)
                            ((IList)result).Add(DeserializeObject(objectType,databaseMetaData));
                        return result;
                    }
                    if(null != TYPE.GetInterface(typeof(IDictionary).ToString()))
                    {
                        //result = Activator.CreateInstance(TYPE);
                        //Type keyType = null;
                        //var valueType = Deserialize2Dictionary(TYPE,out keyType);
                        //foreach(KeyValuePair<string,DatabaseMetaData> pair in m_metaDataDic)
                        //{
                        //    var databaseMetaData = pair.Value;
                        //    object key = DeserializeObject(keyType,pair.Key);
                        //    object value = DeserializeObject(valueType,databaseMetaData);
                        //    ((IDictionary)result).Add(key,value);
                        //}
                        //return result;
                    }
                    // type.IsObject == true
                    {
                        result = Activator.CreateInstance(TYPE);
                        SqliteRow metaData = DATA as SqliteRow;
                        if(null != metaData)
                        {
                            ClassInfo classInfo = null;
                            if(!m_classInfoDic.TryGetValue(TYPE,out classInfo))
                            {
                                classInfo = new ClassInfo();
                                var properties = TYPE.GetProperties();
                                foreach(var propertyInfo in properties)
                                {
                                    if(!classInfo.ContainProperty(propertyInfo))
                                    {
                                        var uSqlite = propertyInfo.GetCustomAttributes(typeof(USqliteSerializeAttribute),true);
                                        if(uSqlite.Length > 0)
                                            classInfo.AddPropertyInfo(propertyInfo);
                                    }
                                }
                                var fields = TYPE.GetFields();
                                foreach(var fieldInfo in fields)
                                {
                                    if(!classInfo.ContainField(fieldInfo))
                                    {
                                        var uSqlite = fieldInfo.GetCustomAttributes(typeof(USqliteSerializeAttribute),true);
                                        if(uSqlite.Length > 0)
                                            classInfo.AddFieldInfo(fieldInfo);
                                    }
                                }
                                m_classInfoDic.Add(TYPE,classInfo);
                            }
                            foreach(var property in classInfo.properties)
                            {
                                foreach (SqliteCell cellData in metaData.cellData)
                                {
                                    if(property.name.Equals(cellData.upperName))
                                    {
                                        var value = DeserializePropertyField(property.propertyInfo.PropertyType,cellData.value);
                                        property.propertyInfo.SetValue(result,value,null);
                                        break;
                                    }
                                }
                            }
                            foreach(var field in classInfo.fields)
                            {
                                foreach (SqliteCell cellData in metaData.cellData)
                                {
                                    if(field.name.Equals(cellData.upperName))
                                    {
                                        var value = DeserializePropertyField(field.fieldInfo.FieldType, cellData.value);
                                        field.fieldInfo.SetValue(result,value);
                                        break;
                                    }
                                }
                            }
                        }
                        return result;
                    }
                });
            }
            m_deserializeFunc.TryGetFunc(type,out sqliteDeserializeFunc);
            result = sqliteDeserializeFunc(type,data);
            return result;
        }

        private object DeserializePropertyField(Type type,object data)
        {
            SqliteDeserializeFunc sqliteDeserializeFunc = null;
            m_deserializeFunc.TryGetFunc(type,out sqliteDeserializeFunc);
            if(null != sqliteDeserializeFunc)
            {
                var result = sqliteDeserializeFunc(type,data);
                return result;
            }
            return null;
        }

        private Type Deserialize2Array(Type type)
        {
            return type.GetElementType();
        }

        private Type Deserialize2IList(Type type)
        {
            var propertyInfo = type.GetProperty("Item");
            if (null == propertyInfo)
                return null;
            ParameterInfo[] parameters = propertyInfo.GetIndexParameters();
            if(parameters.Length == 1 && parameters[0].ParameterType == typeof(int))
            {
                var classType = propertyInfo.PropertyType;
                return classType;
            }
            return null;
        }

        private Type Deserialize2Dictionary(Type type,out Type keyType)
        {
            Type valueType = null;
            keyType = null;
            Type[] arguments = type.GetGenericArguments();
            keyType = arguments[0];
            valueType = arguments[1];
            return valueType;
        }
    }
}