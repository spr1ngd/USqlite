
using System;
using System.Linq.Expressions;
using Mono.Data.Sqlite;

namespace USqlite
{
    public enum SqlCommandType
    {
        None,
        Select,
        Insert,
        Delete,
        Update
    }

    public class BaseCommand
    {
        private readonly SqliteConnection m_sqliteConnection = null;
        protected string m_commandText = string.Empty;
        protected string commandText { get { return string.Format("{0} {1} {2}",m_commandText,conditionCommand,orderbyCommand); } }

        protected string tableName;
        protected string conditionCommand = string.Empty;
        protected string orderbyCommand = string.Empty;

        public BaseCommand( SqliteConnection connection ,string tableName )
        {
            this.m_sqliteConnection = connection;
            this.tableName = tableName;
        }

        public virtual SqliteDataReader Execute()
        {
            var command = m_sqliteConnection.CreateCommand();
            command.CommandText = commandText;
            UnityEngine.Debug.Log(commandText);
            try
            {
                return command.ExecuteReader();
            }
            catch(Exception @exception)
            {
                throw new USqliteException("Read data from database exception",@exception.InnerException);
            }
        }

        public virtual BaseCommand Where(string condition)
        {
            if (!string.IsNullOrEmpty(condition))
            {
                if (string.IsNullOrEmpty(conditionCommand)) conditionCommand = string.Format("WHERE {0}",condition);
                else conditionCommand += string.Format("AND {0}",condition);
            }
            return this;
        }

        public virtual BaseCommand Where<T>( Expression<Func<T,bool>> whereExpression )
        {
            var condition = whereExpression.Body.ToString();
            if (condition.Contains("("))
            {
                int tagStart = condition.IndexOf("(", StringComparison.Ordinal) + 1;
                int tagEnd = condition.IndexOf(".", StringComparison.Ordinal);
                string keyPoint = condition.Substring(tagStart, tagEnd);
                condition = condition.Replace(keyPoint + ".", "");
            }
            else
            {
                int tagEnd = condition.IndexOf(".", StringComparison.Ordinal);
                string keyPoint = condition.Substring(0, tagEnd);
                condition = condition.Replace(keyPoint + ".","");
            }
            UnityEngine.Debug.Log(condition);
            return Where(condition);
        }

        public virtual BaseCommand OrderBy(string columnName)
        {
            if(!string.IsNullOrEmpty(columnName))
            {
                if(string.IsNullOrEmpty(orderbyCommand)) orderbyCommand = string.Format("ORDER BY {0}",columnName);
                else orderbyCommand += string.Format(", {0}",columnName);
            }
            return this;
        }

        public virtual BaseCommand OrderBy<T>( Expression<Func<T,bool>> orderByExpression )
        {
            return this;
        }
    }
}