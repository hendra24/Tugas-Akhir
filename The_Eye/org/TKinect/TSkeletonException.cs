using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.TKinect
{
    [Serializable]
    class TSkeletonException : Exception
    {
        public TSkeletonException() : base() { }
        public TSkeletonException(string message) : base(message) { }
        public TSkeletonException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an 
        // exception propagates from a remoting server to the client.  
        protected TSkeletonException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
