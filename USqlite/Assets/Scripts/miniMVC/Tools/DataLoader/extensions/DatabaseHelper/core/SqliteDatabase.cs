
using System.Data;
using System.Data.Common;
using Mono.Data.Sqlite;

namespace miniMVC.USqlite
{
    public delegate void SqliteTransactionDelegate();

    // todo 添加类Linq语句提升调用连贯性

    /// <summary>
    /// 建立与Sqlite数据库的连接
    /// </summary>
    public class SqliteDatabase
    {
        private readonly string dbPath = null;
        private SqliteConnection connection = null;
        private SqliteCommand command = null;
        private SqliteTransaction transaction = null;
        private SqliteDataReader dbReader = null;

        /// <summary>
        /// 构造与Sqlite数据库实例的连接
        /// </summary>
        /// <param name="dbPath"></param>
        public SqliteDatabase( string dbPath )
        {
            this.dbPath = dbPath;
            OpenConnection();
        }

        /// <summary>
        /// 建立连接
        /// </summary>
        private void OpenConnection()
        {
            if (string.IsNullOrEmpty(dbPath))
            {
                UnityEngine.Debug.Log(string.Format("数据库文件路径 :[{0}] 异常 ",dbPath));
                return;
            }
            try
            {
                connection = new SqliteConnection("data source="+ dbPath);
                connection.Open();
                UnityEngine.Debug.Log(string.Format("<color=red>成功连接到数据库 [{0}] </color>",dbPath));
            }
            catch(DbException exception)
            {
                UnityEngine.Debug.LogError(exception);
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnection()
        {
            if(null != connection) connection.Close();
            if(null != command) command.Cancel();
            if(null != dbReader) dbReader.Close();
            transaction = null;
            connection = null;
            command = null;
            dbReader = null;
        }

        #region table functions 创建表 | 删除表 等表操作

        public void BeginTransaction( SqliteTransactionDelegate action )
        {
            transaction = connection.BeginTransaction();
            try
            {
                if (null != action)
                {
                    action();
                    transaction.Commit();
                }
            }
            catch (SqliteException exception)
            {
                transaction.Rollback();
                UnityEngine.Debug.LogError(exception);
            }
            transaction.Dispose();
        }

        public SqliteDataReader CreateTable(string tableName,string[] columeNames,string[] columeTypes)
        {
            string sql = string.Format("CREATE TABLE {0} ( {1} {2}",tableName,columeNames[0],columeTypes[0]);
            for (int i = 1; i < columeNames.Length; i++)
            {
                sql += "," + columeNames[i] + " " + columeTypes[i];
            }
            sql += " )";
            return ExecuteQuery(sql,null);
        }

        public void DeleteTable( string tableName )
        {
            string sql = string.Format("DROP TABLE {0}",tableName);
            ExecuteQuery(sql,null);
        }

        #endregion

        // todo 重构到SqliteTable中
        #region database base functions 增 | 删 | 改 | 查

        #region Query
        
        public SqliteDataReader Query(string tableName,string[] columeNames = null,string condition = null)
        {
            string columes = "";
            if (null != columeNames)
            {
                columes = columeNames[0];
                for(int i = 1; i < columeNames.Length; i++)
                    columes += "," + columeNames[i];
                columes = columes.Trim();
            }
            string sql = string.Format("SELECT {0} FROM {1} {2}",
                string.IsNullOrEmpty(columes) ? "*" : columes,
                tableName,
                string.IsNullOrEmpty(condition) ? "" : "WHERE " + condition);
            return ExecuteQuery(sql,null);
        }

        #endregion

        #region Insert

        /// <summary>
        /// 数据不安全的插入操作
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columeNames"></param>
        /// <param name="columeValues"></param>
        /// <returns></returns>
        public SqliteDataReader Insert(string tableName,string[] columeNames,string[] columeValues)
        {
            if (columeNames.Length != columeValues.Length)
            {
                UnityEngine.Debug.LogError("参数不匹配");
                return null;
            }
            string columes = columeNames[0];
            for(int i = 1; i < columeNames.Length; i++)
                columes += "," + columeNames[i];
            string values = columeValues[0];
            for (int i = 1; i < columeNames.Length; i++)
                values += "," + columeValues[i];
            string sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",tableName,columes,values);
            return ExecuteQuery(sql,null);
        }

        /// <summary>
        /// 数据不安全的插入操作
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="culumeValues"></param>
        /// <returns></returns>
        public SqliteDataReader Insert( string tableName,string[] culumeValues )
        {
            if (null == culumeValues || culumeValues.Length <= 0)
            {
                UnityEngine.Debug.Log("数据为空，无法导入数据库");
                //throw new ArgumentNullException();
                return null;
            }
            string values = culumeValues[0];
            for(int i = 1; i < culumeValues.Length; i++)
                values += "," + culumeValues[i];
            string sql = string.Format("INSERT INTO {0} VALUES ({1})",tableName,values);
            return ExecuteQuery(sql, null);
        }

        public SqliteDataReader Insert( string tableName )
        {
            SqliteParameter[] @params = new SqliteParameter[]{};
            string sql = string.Format("INSERT INTO {0}",tableName);
            return ExecuteQuery(sql, @params);
        }

        #endregion

        #region Update

        public void Update(string tableName , string[] columeNames,string[] columeValues,string conditionKey,string conditionOperate,string conditionValue)
        {
            string condition = null;
            if (!string.IsNullOrEmpty(conditionKey) &&
                !string.IsNullOrEmpty(conditionOperate) &&
                !string.IsNullOrEmpty(conditionValue))
                condition = string.Format("{0} {1} {2}", conditionKey, conditionOperate, conditionValue);
            Update(tableName,columeNames,columeValues,condition);
        }

        public SqliteDataReader Update(string tableName,string[] columeNames,string[] columeValues,string condition = null)
        {
            string setValue = string.Format("{0} = {1}", columeNames[0], columeValues[0]);
            for (int i = 1; i < columeNames.Length; i++)
                setValue += "," + string.Format("{0} = {1}", columeNames[i], columeValues[i]);
            string conditionSql = !string.IsNullOrEmpty(condition)? string.Format("WHERE {0}",condition):"";
            string sql = string.Format("UPDATE {0} SET {1} {2}",tableName,setValue,conditionSql);
            return ExecuteQuery(sql, null);
        }

        #endregion

        #region Delete

        public void Delete(string tableName,string conditionKey,string conditionOperate,string conditionValue)
        {
            string condition = null;
            if(!string.IsNullOrEmpty(conditionKey) &&
               !string.IsNullOrEmpty(conditionOperate) &&
               !string.IsNullOrEmpty(conditionValue))
                condition = string.Format("{0} {1} {2}",conditionKey,conditionOperate,conditionValue);
            Delete(tableName,condition);
        }

        public SqliteDataReader Delete(string tableName,string condition)
        {
            string sql = string.Format("DELETE FROM {0} WHERE {1}", tableName,condition);
            return ExecuteQuery(sql, null);
        }

        #endregion

        private SqliteDataReader ExecuteQuery( string sql ,SqliteParameter[] sqliteParam)
        {
            CheckConnection();
            // todo 多次构造的问题
            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //UnityEngine.Debug.Log(string.Format("Query : <color=#FF00FF>[{0}]</color>",sql));
            SqliteDataReader result = null;
            try
            {
                command = connection.CreateCommand();
                command.CommandText = sql;
                if(null != sqliteParam && sqliteParam.Length > 0)
                    command.Parameters.AddRange(sqliteParam);
                result =  command.ExecuteReader();
            }
            catch (SqliteException exception)
            {
                UnityEngine.Debug.LogError(exception);
            }
            //watch.Stop();
            return result;
        }

        private void CheckConnection()
        {
            // 重启已经关闭或断开的数据库连接
            if (connection.State.Equals(ConnectionState.Closed) || connection.State.Equals(ConnectionState.Broken))
                connection.Open();
        }

        #endregion
    }
}