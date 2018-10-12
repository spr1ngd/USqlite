
using System;

namespace USqlite
{
    public class USqliteException : Exception
    {
        public USqliteException():base()
        {

        }

        public USqliteException(string message):base(message)
        {

        }

        public USqliteException(string message,Exception exception) : base(message,exception)
        {

        }
    }
}