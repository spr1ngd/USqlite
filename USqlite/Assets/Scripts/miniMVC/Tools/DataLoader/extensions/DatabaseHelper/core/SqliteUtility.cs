
using System;
using System.Diagnostics;
using Mono.Data.Sqlite;
using System.IO;
using USqlite;
using Console = UnityEngine.Debug;

namespace miniMVC.USqlite
{
    public class SqliteUtility
    {
        private static SqliteCustomSerializeFunc m_customSerializeFunc = null;
        private static SqliteCustomSerializeFunc customSerializeFunc
        {
            get
            {
                if( null == m_customSerializeFunc )
                    m_customSerializeFunc = new SqliteCustomSerializeFunc();
                return m_customSerializeFunc;
            }
        }

        private static SerializeFactory m_serializeFactory = null;
        private static SerializeFactory serializeFactory
        {
            get
            {
                if( null == m_serializeFactory )
                    m_serializeFactory = new SerializeFactory(customSerializeFunc);
                return m_serializeFactory;
            }
        }

        private static DeserializeFactory m_deserializeFactory = null;
        private static DeserializeFactory deserializeFactory
        {
            get
            {
                if( null == m_deserializeFactory )
                    m_deserializeFactory = new DeserializeFactory(customSerializeFunc);
                return m_deserializeFactory;
            }
        }

        private static SqliteDatabase m_currentDB = null;

        #region database functions 数据库基本操作

        public static void RegisterCustomSerializeFunc(Type type,SqliteCustomSerializeFunc.CustomSerializeFunc serialize = null,SqliteCustomSerializeFunc.CustomDeserializeFunc deserialize = null)
        {
            customSerializeFunc.RegisterCustomSerializeFunc(type,serialize,deserialize);
        }

        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <param name="dbPath"></param>
        public static void OpenDatabase(string dbPath)
        {
            RegisterCustomSerializeFunc(typeof(UnityEngine.Vector3),
                vector =>
                {
                    UnityEngine.Vector3 vector3 = (UnityEngine.Vector3)vector;
                    return string.Format(@"""{0},{1},{2}""",vector3.x,vector3.y,vector3.z);
                },
                strValue => {
                    string[] array = strValue.Split(',');
                    UnityEngine.Vector3 value = new UnityEngine.Vector3();
                    value.x = float.Parse(array[0]);
                    value.y = float.Parse(array[1]);
                    value.z = float.Parse(array[2]);
                    return value;
                });
            RegisterCustomSerializeFunc(typeof(UnityEngine.Quaternion),
                quaternion =>
                {
                    UnityEngine.Quaternion q4 = (UnityEngine.Quaternion)quaternion;
                    return string.Format(@"""{0},{1},{2},{3}""",q4.x,q4.y,q4.w,q4.z);
                },
                strValue => {
                    string[] array = strValue.Split(',');
                    UnityEngine.Quaternion value = new UnityEngine.Quaternion();
                    value.x = float.Parse(array[0]);
                    value.y = float.Parse(array[1]);
                    value.z = float.Parse(array[2]);
                    value.w = float.Parse(array[3]);
                    return value;
                });
            
            m_currentDB = new SqliteDatabase(dbPath);
            Console.Log(string.Format("<color=green> 成功连接到 [{0}] 数据库</color>",dbPath));
        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        /// <param name="dbPath"></param>
        public static void CloseDatabase(string dbPath)
        {
            if(null != m_currentDB)
            {
                m_currentDB.CloseConnection();
                Console.Log(string.Format("<color=red> 成功关闭 [{0}] 数据库</color>",dbPath));
                m_currentDB = null;
            }
            else
            {
                Console.Log(string.Format("<color=red>暂无数据库连接</color>"));
            }
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="dbPath"></param>
        public static void CreateDatabase(string dbPath)
        {
            if(File.Exists(dbPath))
            {
                Console.Log(string.Format("The database <color=red>[{0}]</color> is exist.",dbPath));
            }
            else
            {
                SqliteConnection.CreateFile(dbPath);
                Console.Log(string.Format("The database <color=red>[{0}]</color> created successful.",dbPath));
            }
        }

        /// <summary>
        /// 移除数据库
        /// </summary>
        /// <param name="dbPath"></param>
        public static void DropDatabase(string dbPath)
        {
            if(File.Exists(dbPath))
            {
                try
                {
                    File.Delete(dbPath);
                    Console.Log(string.Format("The database <color=red>[{0}]</color> has beed deleted.",dbPath));
                }
                catch(IOException e)
                {
                    Console.Log(e);
                    throw;
                }
            }
            else
            {
                Console.Log(string.Format("The database <color=red>[{0}]</color> is not exist.",dbPath));
            }
        }

        #endregion

        #region table functions 表基本操作

        /// <summary>
        /// 启动事务(推荐将数据库操作逻辑加入到事务处理中，以提升处理速度以及操作异常回滚)
        /// </summary>
        /// <param name="action"></param>
        public static void BeginTransaction(SqliteTransactionDelegate action)
        {
            m_currentDB.BeginTransaction(action);
        }

        public static void CreateTable(string tableName,string[] columeNames,string[] columeTypes)
        {
            m_currentDB.CreateTable(tableName,columeNames,columeTypes);
        }

        public static void DeleteTable(string tableName)
        {
            m_currentDB.DeleteTable(tableName);
        }

        #endregion

        #region base functions 增|删|改|查

        public static SqliteDataReader Query(string tableName,string[] columeNames = null,string condition = null)
        {
            return m_currentDB.Query(tableName,columeNames,condition);
        }

        public static SqliteDataReader Insert(string tableName,string[] culumeValues)
        {
            return m_currentDB.Insert(tableName,culumeValues);
        }

        public static SqliteDataReader Update(string tableName,string[] columeNames,string[] columeValues,string condition = null)
        {
            return m_currentDB.Update(tableName,columeNames,columeValues,condition);
        }

        public static SqliteDataReader Delete(string tableName,string condition)
        {
            return m_currentDB.Delete(tableName,condition);
        }

        public static SqliteDataReader Delete(string tableName,object objectData,string condition = null)
        {
            SqliteRow metaData = objectData as SqliteRow;
            if (null != metaData && null != metaData.primayKey ) // 利用已有数据的主键进行数据库数据删除，以保证唯一性
                Delete(tableName,
                    string.Format("{0} = {1}{2}",
                    metaData.primayKey.name,
                    metaData.primayKey,
                    string.IsNullOrEmpty(condition)?"":string.Format(" AND {0}",condition)));
            return null;
        }

        public static SqliteDataReader InsertObject(string tableName,object obj)
        {
            serializeFactory.Clear();
            serializeFactory.SerializeObject(obj.GetType(),obj,obj);
            foreach (var metaData in serializeFactory.write2Database)
                Insert(tableName,metaData.columeValues);
            Console.Log(string.Format("<color=red> {0}行数据,导入数据库</color>",serializeFactory.write2Database.Count));
            return null;
        }

        public static SqliteDataReader UpdateObject(string tableName,object obj,string condition = null)
        {
            serializeFactory.Clear();
            serializeFactory.SerializeObject(obj.GetType(),obj,obj);
            foreach (SqliteRow metaData in serializeFactory.write2Database)
            {
                string[] columeNames = metaData.columeNames;
                string[] columeValues = metaData.columeValues;
                Update(tableName,columeNames,columeValues,string.Format("{0} = {1}",metaData.primayKey.name,metaData.primayKey.value));
            }
            return null;
        }

        public static SqliteDataReader DeleteObject(string tableName,object obj , string condition = null)
        {
            serializeFactory.write2Database.Clear();
            serializeFactory.SerializeObject(obj.GetType(),obj,obj);
            foreach (var metaData in serializeFactory.write2Database)
                Delete(tableName,metaData,condition);
            return null;
        }
        
        public static T QueryObject<T>(string tableName,string[] columeNames = null,string condition = null)
        {
            deserializeFactory.Clear();
            SqliteTable dbTable = new SqliteTable();
            SqliteCell rawCell = new SqliteCell();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            SqliteDataReader reader = m_currentDB.Query(tableName,columeNames,condition);
            while(reader.Read())
            {
                var metaData = new SqliteRow();
                for(int rowId = 0; rowId < reader.FieldCount; rowId++)
                {
                    var celldata = (SqliteCell)rawCell.Clone();
                    if (dbTable.ContainRow(rowId))
                    {
                        var row = dbTable.GetRow(rowId);
                        celldata.name = row.rowName;
                        celldata.type = row.rowType;
                        celldata.upperName = row.rowUpperName;
                        celldata.value = row.GetValue(rowId);
                    }
                    else
                    {
                        var name = reader.GetName(rowId);
                        var upperName = name.ToUpper();
                        var type = reader.GetProviderSpecificFieldType(rowId);
                        celldata.name = name;
                        celldata.upperName = upperName;
                        celldata.type = type;
                        Func<int,object> getValue = null;
                        if(celldata.type == typeof(Int32))
                            getValue = ROWID => reader.GetInt32(ROWID);
                        else if(celldata.type == typeof(Int64))
                            getValue = ROWID => reader.GetInt32(ROWID);
                        else if(celldata.type == typeof(string))
                            getValue = ROWID => reader.GetString(ROWID);
                        else if(celldata.type == typeof(bool))
                            getValue = ROWID => reader.GetBoolean(ROWID);
                        else if (celldata.type == typeof(float))
                            getValue = ROWID => reader.GetFloat(ROWID);
                        if( null != getValue )
                            celldata.value = getValue(rowId);
                        dbTable.AddRow(rowId,type,name,upperName,getValue);
                    }
                    metaData.AddCell(celldata);
                }
                deserializeFactory.dbMetadata.Add(metaData);
            }
            watch.Stop();
            Console.Log(string.Format("读取耗时:{0}",watch.Elapsed));

            watch.Reset();
            watch.Start();
            object deserializeData = deserializeFactory.dbMetadata;
            if(deserializeFactory.dbMetadata.Count <= 0)
            {
                Console.Log("未查训到相关数据");
                return default(T);
            }
            if(deserializeFactory.dbMetadata.Count <= 1)
            {
                deserializeData = deserializeFactory.dbMetadata[0];
                if(deserializeFactory.dbMetadata[0].cellData.Count <= 0)
                {
                    Console.Log("查询数据为空");
                    return default(T);
                }
                if(deserializeFactory.dbMetadata[0].cellData.Count <= 1)
                    deserializeData = deserializeFactory.dbMetadata[0].cellData[0];
            }
            var result = deserializeFactory.DeserializeObject(typeof(T),deserializeData);
            reader.Close();
            watch.Stop();
            Console.Log(string.Format("反序列化耗时:{0}",watch.Elapsed));
            return (T)result;
        }

        #endregion
    }

    public class Sqlite3
    {
        public void Query( string tableName )
        {
            var data = new QueryCommand(null).Where("id<5").Descending().Execute();
        }
    }
}