
using miniMVC.USqlite;

namespace USqlite
{
    public static class Sqlite3
    {
        private static SqliteDatabase m_database = null;

        public static void Open(string dbPath)
        {
            m_database = new SqliteDatabase(dbPath);
            Orm.RegisterCustomSerializeFunc(typeof(UnityEngine.Vector3),
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
            Orm.RegisterCustomSerializeFunc(typeof(UnityEngine.Quaternion),
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
        }

        public static void Close()
        {
            m_database.CloseConnection();
            UnityEngine.Debug.Log("数据库关闭");
        }

        /// <summary>
        /// 查询数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DatabaseTable<T> Table<T>()
        {
            return new DatabaseTable<T>(m_database.connection);
        }

        /// <summary>
        /// 新建数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void CreateTable<T>(string tableName = null)
        {
            new DatabaseTable<T>(m_database.connection).Create(tableName);
        }

        /// <summary>
        /// 删除数据表
        /// </summary>
        public static void DropTable(string tableName)
        {
            new DatabaseTable<int>(m_database.connection).Drop(tableName);
        }

        /// <summary>
        /// 删除数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void DropTable<T>()
        {
            new DatabaseTable<T>(m_database.connection).Drop();
        }
    }
}