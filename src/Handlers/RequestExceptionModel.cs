using System.Collections.Generic;

namespace Handlers
{
    public class RequestExceptionModel
    {

        public RequestExceptionModel(List<string> requestValidationErrors)
        {
            Reasons = requestValidationErrors;
        }

        public List<string> Reasons { get; internal set; }
    }
}
