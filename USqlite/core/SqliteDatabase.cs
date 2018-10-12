
using System.Data.Common;
using Mono.Data.Sqlite;

namespace USqlite
{
    public class SqliteDatabase
    {
        private readonly string dbPath = null;
        private SqliteConnection m_connection = null;
        private SqliteCommand command = null;
        private SqliteDataReader dbReader = null;

        public SqliteConnection connection { get { return m_connection; }}

        public SqliteDatabase( string dbPath )
        {
            this.dbPath = dbPath;
            OpenConnection();
        }

        private void OpenConnection()
        {
            if (string.IsNullOrEmpty(dbPath))
            {
                //UnityEngine.Debug.Log(string.Format("数据库文件路径 :[{0}] 异常 ",dbPath));
                return;
            }
            try
            {
                m_connection = new SqliteConnection("data source="+ dbPath);
                m_connection.Open();
                //UnityEngine.Debug.Log(string.Format("<color=green>成功连接到数据库 [{0}] </color>",dbPath));
            }
            catch(DbException exception)
            {
                throw exception;
            }
        }

        public void CloseConnection()
        {
            if(null != m_connection) m_connection.Close();
            if(null != command) command.Cancel();
            if(null != dbReader) dbReader.Close();
            m_connection = null;
            command = null;
            dbReader = null;
            //UnityEngine.Debug.Log(string.Format("<color=red>关闭数据库连接 [{0}] </color>",dbPath));
        }
    }
}