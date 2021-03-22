using System;
using System.Runtime.Serialization;

namespace Api
{
    [Serializable]
    class ServicesSetupException : Exception
    {
        public ServicesSetupException()
        {
        }

        public ServicesSetupException(string message) : base(message)
        {
        }

        public ServicesSetupException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ServicesSetupException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}