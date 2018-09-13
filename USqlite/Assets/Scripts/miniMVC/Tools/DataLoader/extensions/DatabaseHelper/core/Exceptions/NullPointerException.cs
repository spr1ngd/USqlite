
using System;

namespace USqlite
{
    public class NullPointerException : USqliteException
    {
        public NullPointerException() : base()
        {

        }

        public NullPointerException(string message) : base(message)
        {

        }

        public NullPointerException(string message,Exception exception) : base(message,exception)
        {

        }
    }
}