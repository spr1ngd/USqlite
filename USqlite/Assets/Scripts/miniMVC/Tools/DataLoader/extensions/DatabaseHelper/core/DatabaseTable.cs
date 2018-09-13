
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mono.Data.Sqlite;

namespace USqlite
{
    public class DatabaseTable<T> : IEnumerable<T>
    {
        private readonly SqliteConnection m_connection = null;
        private BaseCommand m_command = null;
        private SqliteDataReader m_dataReader = null;

        public DatabaseTable(SqliteConnection connection)
        {
            this.m_connection = connection;
        }

        #region Table Command

        public DatabaseTable<T> Create( string tableName )
        {
            m_command = new CreateTableCommand(m_connection,typeof(T),tableName);
            ExecuteCommand();
            return this;
        }

        public DatabaseTable<T> Drop( string tableName = null )
        {
            m_command = new DropTableCommand(m_connection,typeof(T),tableName);
            ExecuteCommand();
            return this;
        }
        
        #endregion

        #region Basic Command

        public DatabaseTable<T> Select()
        {
            return Select(typeof(T).Name);
        }

        public DatabaseTable<T> Select( string tableName )
        {
            m_command = new SelectCommand(m_connection,typeof(T),tableName);
            return this;
        }

        public DatabaseTable<T> Insert(IList<T> objs)
        {
            var transaction = m_connection.BeginTransaction();
            for(int i = 0; i < objs.Count - 1; i++)
                Insert(objs[i]);
            transaction.Commit();
            return Insert(objs[objs.Count - 1]);
        }

        public DatabaseTable<T> Insert(string tableName,IList<T> objs)
        {
            var transaction = m_connection.BeginTransaction();
            for(int i = 0; i < objs.Count - 1; i++)
                Insert(tableName,objs[i]);
            transaction.Commit();
            return Insert(tableName,objs[objs.Count - 1]);
        }

        public DatabaseTable<T> Insert(T obj)
        {
            return Insert(typeof(T).Name,obj);
        }

        public DatabaseTable<T> Insert(string tableName,T obj)
        {
            m_command = new InsertCommand<T>(m_connection,typeof(T),obj,tableName);
            ExecuteCommand();
            return this;
        }

        public DatabaseTable<T> Update(Expression<Func<T,string[]>> setColumn,object[] values)
        {
            return Update(typeof(T).Name,setColumn,values);
        }

        public DatabaseTable<T> Update( string tableName,Expression<Func<T,string[]>> setColumn,object[] values)
        {
            m_command = new UpdateCommand<T>(m_connection,typeof(T),tableName);
            (m_command as UpdateCommand<T>).Set(setColumn,values);
            return this;
        }

        public DatabaseTable<T> Delete()
        {
            return Delete(typeof(T).Name);
        }

        public DatabaseTable<T> Delete( string tableName )
        {
            m_command = new DeleteCommand(m_connection,typeof(T),tableName);
            return this;
        }

        #endregion

        public DatabaseTable<T> Where(Expression<Func<T,bool>> whereExpression)
        {
            m_command.Where(whereExpression);
            return this;
        }

        public DatabaseTable<T> OrderBy()
        {
            return this;
        }

        private void ExecuteCommand()
        {
            m_dataReader = m_command.Execute();
        }

        public void ExecuteNoQuery()
        {
            ExecuteCommand();
        }

        public IEnumerable<T> Execute()
        {
            yield return default(T);
        }

        public List<T> Execute2List()
        {
            ExecuteCommand();
            return Orm.Mapping2List<T>(m_dataReader);
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}