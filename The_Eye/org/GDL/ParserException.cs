using System;
using System.Collections.Generic;
using System.Text;

namespace org.GDL
{
    [Serializable()]
    class ParserException : Exception
    {
        public int col = 0;
        public int ln = 0;
        public ParserException() : base() { }
        public ParserException(string message, int col, int ln) : base(message) 
        {
            this.col = col;
            this.ln = ln;
        }
        public ParserException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected ParserException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) { }
    }
}
