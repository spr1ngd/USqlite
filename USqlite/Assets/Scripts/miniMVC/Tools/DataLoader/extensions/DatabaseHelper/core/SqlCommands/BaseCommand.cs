
using System;
using Mono.Data.Sqlite;

namespace USqlite
{
    public class BaseCommand
    {
        private readonly SqliteConnection m_sqliteConnection = null;
        protected string commandText = null;


        public BaseCommand( SqliteConnection connection )
        {
            this.m_sqliteConnection = connection;
        }

        public virtual SqliteDataReader Execute()
        {
            var command = m_sqliteConnection.CreateCommand();
            command.CommandText = commandText;
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
            return this;
        }

        public virtual BaseCommand Ascending()
        {
            return this;
        }

        public virtual BaseCommand Descending()
        {
            return this;
        }
    }
}