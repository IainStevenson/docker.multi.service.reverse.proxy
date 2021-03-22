using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Handlers
{

  
    public class RequestHandler<TRequestModel, TResponseModel> : IRequestHandler<TRequestModel, TResponseModel>
        where TRequestModel : class
        where TResponseModel : class
    {
        private readonly IEnumerable<IHttpActionRequestHandler<TRequestModel, TResponseModel>> _handlers;
        public RequestHandler(IEnumerable<IHttpActionRequestHandler<TRequestModel, TResponseModel>> handlers)
        {
            _handlers = handlers;
        }
        public async Task<ResponseMessage<TResponseModel>> Handle(RequestMessage<TRequestModel> request)
        {
            var handler = _handlers.FirstOrDefault(x => x.Action == request.RequestType);
            if (handler == null) throw new ServicesSetupException($"No handler provided for Http Method {request.RequestType}");
            return await handler.Handle(request);
        }

    }

    /// <summary>
    /// Manages many <see cref="IHttpStatusCodeResponseHandler<TResponse>"/> handlers to handle the processed response. 
    /// Implementations:
    /// OK 
    /// </summary>
    /// <typeparam name="TResponseModel">The type of the response to handle</typeparam>
    public class ResponseHander<TResponseModel> : IResponseHander<TResponseModel>
        where TResponseModel : class
    {
        private readonly IEnumerable<IHttpStatusCodeResponseHandler<TResponseModel>> _handlers;

        public ResponseHander(IEnumerable<IHttpStatusCodeResponseHandler<TResponseModel>> handlers)
        {
            _handlers = handlers;
        }
        /// <summary>
        /// Handle the incoming response. If no handler is found it will throw an exception of type <see cref="ServicesSetupException"/>
        /// </summary>
        /// <param name="response">The response to handle</param>
        /// <returns>An <see cref="IActionResult"/> from the appropriate handler</returns>
        public async Task<IActionResult> Handle(ResponseMessage<TResponseModel> response)
        {
            var handler = _handlers.FirstOrDefault(x => x.StatusCode == response.StatusCode);
            if (handler == null) throw new ServicesSetupException($"No handler provided for Http Status Code {response.StatusCode}");
            return await handler.Handle(response);
        }
    }

}
