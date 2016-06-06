using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SCMS.CoreBusinessLogic
{
    public class SCMSException : Exception
    {
       
        public SCMSException()
        {
        }

        public SCMSException(string message)
            : base(message)
        {
        }

        
        public SCMSException(string messageFormat, params object[] args)
			: base(string.Format(messageFormat, args))
		{
		}

        protected SCMSException(SerializationInfo
            info, StreamingContext context)
            : base(info, context)
        {
        }

        public SCMSException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
