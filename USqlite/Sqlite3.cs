
namespace USqlite
{
    public static class Sqlite3
    {
        private static SqliteDatabase m_database = null;

        public static void Open(string dbPath)
        {
            m_database = new SqliteDatabase(dbPath);
        }

        public static void Close()
        {
            m_database.CloseConnection();
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