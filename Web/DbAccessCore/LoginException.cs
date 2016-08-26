using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbAccessCore
{
    [Serializable]
    public class LoginException : Exception
    {
        public LoginException() { }
        public LoginException(string message) : this(message, null) { }
        public LoginException(string message, Exception inner) : base(message, inner) { }
        protected LoginException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}