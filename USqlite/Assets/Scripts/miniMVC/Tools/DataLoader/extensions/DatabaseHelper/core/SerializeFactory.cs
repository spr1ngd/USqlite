
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace miniMVC.USqlite
{
    // todo 优化序列化算法
    public class SerializeFactory
    {
        private readonly SqliteCustomSerializeFunc m_customSerializeFunc = null;
        private readonly IList<SqliteRow> m_write2Database = new List<SqliteRow>();
        public IList<SqliteRow> write2Database { get { return m_write2Database; } }

        public SerializeFactory(SqliteCustomSerializeFunc customSerializeFunc )
        {
            m_customSerializeFunc = customSerializeFunc;
        }

        public void Clear()
        {
            m_write2Database.Clear();
        }

        public object SerializeObject(Type type,object obj,object data)
        {
            if(type.IsEnum) // enum
            {
                return SerializePropertyOrField(typeof(string),obj,data);
            }
            else if(type == typeof(int)) // int
            {
                return SerializePropertyOrField(typeof(int),obj,data);
            }
            else if(type == typeof(float)) // float 
            {
                // 转换成string以保留小数点精度
                return SerializePropertyOrField(typeof(string),obj,data);
            }
            else if(type == typeof(double)) // double
            {
                // 转换成string以保留小数点精度
                return SerializePropertyOrField(typeof(string),obj,data);
            }
            else if(type == typeof(string)) // string
            {
                return SerializePropertyOrField(typeof(string),obj,data);
            }
            else if(type == typeof(bool)) // bool
            {
                return SerializePropertyOrField(typeof(bool),obj,data);
            }
            else if(m_customSerializeFunc.ContainSerializeFunc(type)) // custom type
            {
                SqliteCustomSerializeFunc.CustomSerializeFunc serializeFunc = null;
                if(m_customSerializeFunc.TryGetSerializaFunc(type,out serializeFunc))
                {
                    var temp = SerializePropertyOrFieldForCustomType(typeof(string),obj,data);
                    string tempValue = serializeFunc(((SqliteCell)temp).value);
                    string tempName = null;
                    PropertyInfo propertyInfo = obj as PropertyInfo;
                    if(null != propertyInfo)
                        tempName = propertyInfo.Name;
                    FieldInfo fieldInfo = obj as FieldInfo;
                    if(null != fieldInfo)
                        tempName = fieldInfo.Name;
                    return new SqliteCell() { type = typeof(string),name = tempName,value = tempValue };
                }
            }
            else if(type.IsArray) // array 
            {
                Array dataArray = (Array)data;
                foreach(object objectData in dataArray)
                    SerializeObject(objectData.GetType(),objectData,objectData);
            }
            else if(type.IsClass) // object | ilist | idictionary
            {
                if(null != type.GetInterface("System.Collections.IList"))
                {
                    foreach(object objectData in (IList)data)
                        SerializeObject(objectData.GetType(),objectData,objectData);
                }
                if(null != type.GetInterface("System.Collections.IDictionary"))
                {
                    IDictionary objetDic = (IDictionary)data;
                    foreach(object value in objetDic.Values)
                        SerializeObject(value.GetType(),value,value);
                }
                // type.IsObject == true
                {
                    SqliteRow metaData = new SqliteRow();
                    Type dataType = data.GetType();
                    var properties = dataType.GetProperties();
                    foreach(PropertyInfo propertyInfo in properties)
                    {
                        var uSqlite = propertyInfo.GetCustomAttributes(typeof(USqliteSerializeAttribute),true);
                        if(uSqlite.Length > 0)
                        {
                            var cellData = SerializeObject(propertyInfo.PropertyType,propertyInfo,data);
                            metaData.AddCell(cellData as SqliteCell);
                        }
                    }
                    var fields = dataType.GetFields();
                    foreach(FieldInfo fieldInfo in fields)
                    {
                        var uSqlite = fieldInfo.GetCustomAttributes(typeof(USqliteSerializeAttribute),true);
                        if(uSqlite.Length > 0)
                        {
                            var cellData = SerializeObject(fieldInfo.FieldType,fieldInfo,data);
                            metaData.AddCell(cellData as SqliteCell);
                        }
                    }
                    m_write2Database.Add(metaData);
                }
            }
            return null;
        }

        private object SerializePropertyOrField(Type dbDataType,object obj,object data)
        {
            PropertyInfo propertyInfo = obj as PropertyInfo;
            if(null != propertyInfo)
            {
                SqliteCell cellData = new SqliteCell();
                var pkAtt = propertyInfo.GetCustomAttributes(typeof(USqlitePrimaryKeyAttribute),true);
                if(pkAtt.Length > 0)
                    cellData.isPrimaryKey = true;
                cellData.type = dbDataType;
                var tempValue = propertyInfo.GetValue(data,null);
                cellData.value = dbDataType == typeof(string) ? string.Format(@"""{0}""",tempValue) : tempValue;
                cellData.name = propertyInfo.Name;
                return cellData;
            }
            FieldInfo fieldInfo = obj as FieldInfo;
            if(null != fieldInfo)
            {
                SqliteCell cellData = new SqliteCell();
                var pkAtt = fieldInfo.GetCustomAttributes(typeof(USqlitePrimaryKeyAttribute),true);
                if(pkAtt.Length > 0)
                    cellData.isPrimaryKey = true;
                cellData.type = dbDataType;
                var tempValue = fieldInfo.GetValue(data);
                cellData.value = dbDataType == typeof(string) ? string.Format(@"""{0}""",tempValue) : tempValue;
                cellData.name = fieldInfo.Name;
                return cellData;
            }
            return null;
        }

        private object SerializePropertyOrFieldForCustomType(Type dbDataType,object obj,object data)
        {
            PropertyInfo propertyInfo = obj as PropertyInfo;
            if(null != propertyInfo)
            {
                SqliteCell cellData = new SqliteCell();
                var pkAtt = propertyInfo.GetCustomAttributes(typeof(USqlitePrimaryKeyAttribute),true);
                if(pkAtt.Length > 0)
                    cellData.isPrimaryKey = true;
                cellData.type = dbDataType;
                cellData.value = propertyInfo.GetValue(data,null);
                cellData.name = propertyInfo.Name;
                return cellData;
            }
            FieldInfo fieldInfo = obj as FieldInfo;
            if(null != fieldInfo)
            {
                SqliteCell cellData = new SqliteCell();
                var pkAtt = fieldInfo.GetCustomAttributes(typeof(USqlitePrimaryKeyAttribute),true);
                if(pkAtt.Length > 0)
                    cellData.isPrimaryKey = true;
                cellData.type = dbDataType;
                cellData.value = fieldInfo.GetValue(data);
                cellData.name = fieldInfo.Name;
                return cellData;
            }
            return null;
        }
    }
}