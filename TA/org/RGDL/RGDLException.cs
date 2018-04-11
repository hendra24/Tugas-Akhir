using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.RGDL
{
    [Serializable()]
    class RGDLException : Exception
    {
        public RGDLException() : base() { }

        public RGDLException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected RGDLException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) { }
    }
}
