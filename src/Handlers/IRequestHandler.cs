using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;


namespace Handlers
{

    public enum RequestActions
    {
        POST,
        GET,
        PUT,
        DELETE
    }

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

    public class RequestMessage<TRequestModel> where TRequestModel : class
    {

        public Type Caller { get; set; }
        public string RequestType { get; set; }
        public IHeaderDictionary Headers { get; set; }
        public IQueryCollection Query { get; set; }
        public dynamic Parameters { get; set; }
        public TRequestModel Model { get; set; }
        public Guid Owner { get; set; }
        public Guid RequestId { get; set; }
    }

    public class ResponseMessage<TResponseModel>
    {
        public HttpStatusCode StatusCode { get; set; }
        public TResponseModel Model { get; set; }

        public dynamic ExceptionModel { get; set; }
    }

    /// <summary>
    /// The primary handler for a type of request. This handler will process all logic to determine a result and return the required repoose message to be handled by the appropriate response handler.
    /// </summary>
    /// <typeparam name="TRequest">The request to process</typeparam>
    /// <typeparam name="TResponse">the type of response returned to the caller</typeparam>
    public interface IRequestHandler<TRequestModel, TResponseModel>
         where TRequestModel : class
         where TResponseModel : class
    {

        /// <summary>
        /// Handles the request according to rules held in the internal handlers collection for the type of request
        /// </summary>
        /// <param name="request">the request details</param>
        /// <returns>An object of type <see cref="TResponse"/> that contains response results that are to be returned to the caller and instructions on how that is to happen.</returns>
        Task<ResponseMessage<TResponseModel>> Handle(RequestMessage<TRequestModel> request);
    }



    /// <summary>
    /// Abstracts a collection of HttpStatusCodeHandlers to handle the provided response
    /// </summary>
    /// <typeparam name="TResponseModel">The response type to handle</typeparam>
    public interface IResponseHander<TResponseModel> where TResponseModel : class
    {
        /// <summary>
        /// A request response handler
        /// </summary>
        /// <param name="response">The response to handle</param>
        /// <returns> <see cref="IActionResult"/> which is used to directly return from a controller action. </returns>
        /// <example>
        ///  return await _responseHandler.Handle(response);
        /// </example>
        Task<IActionResult> Handle(ResponseMessage<TResponseModel> response);
    }
    /// <summary>
    /// Implentations:
    ///  Action POST   IHttpActionRequestHandler<dynamic, Data.Model.Response.Resource>>
    ///  Action GET    IHttpActionRequestHandler<dynamic, Data.Model.Response.Resource>>
    ///  Action GET    IHttpActionRequestHandler<dynamic, List<Data.Model.Response.Resource>>>
    ///  Action PUT    IHttpActionRequestHandler<dynamic, Data.Model.Response.Resource>>
    ///  Action DELETE IHttpActionRequestHandler<dynamic, Data.Model.Response.Resource>>
    /// </summary>
    /// <typeparam name="TRequestModel"></typeparam>
    /// <typeparam name="TResponseModel"></typeparam>
    public interface IHttpActionRequestHandler<TRequestModel, TResponseModel>
        where TRequestModel : class
        where TResponseModel : class
    {
        string Action { get; set; }
        Task<ResponseMessage<TResponseModel>> Handle(RequestMessage<TRequestModel> request);
    }

    /// <summary>
    /// Handles a single status code required by a response.
    /// Will throw an exception if the response does not match the declared status code.
    /// Implementations:
    /// 200 IHttpStatusCodeResponseHandler<List<Data.Model.Response.Resource>>
    /// 200 IHttpStatusCodeResponseHandler<Data.Model.Response.Resource>
    /// 201 IHttpStatusCodeResponseHandler<Data.Model.Response.Resource>
    /// 204 IHttpStatusCodeResponseHandler<Data.Model.Response.Resource>
    /// 400 IHttpStatusCodeResponseHandler<Data.Model.Response.Resource>
    /// 401 IHttpStatusCodeResponseHandler<Data.Model.Response.Resource>
    /// 404 IHttpStatusCodeResponseHandler<Data.Model.Response.Resource>
    /// 412 IHttpStatusCodeResponseHandler<Data.Model.Response.Resource>
    /// 
    /// </summary>
    /// <typeparam name="TResponseModel">The type of response Model</typeparam>
    public interface IHttpStatusCodeResponseHandler<TResponseModel>
        where TResponseModel : class
    {
        HttpStatusCode StatusCode { get; }
        Task<IActionResult> Handle(ResponseMessage<TResponseModel> response);
    }
}
