
using miniMVC.USqlite;

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
        /// T 为反序列的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DatabaseTable<T> Table<T>()
        {
            return new DatabaseTable<T>(null);
        }
    }
}