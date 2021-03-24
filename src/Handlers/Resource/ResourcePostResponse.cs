using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net;

namespace Handlers.Resource
{
    public class ResourcePostResponse : IResponseAction
    {
        /// <summary>
        /// Determines the resulting action response from the calling controller
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The desired payload model
        /// </summary>
        public Data.Model.Response.Resource Model { get; set; }

        /// <summary>
        /// A collection of resource validation error messages
        /// </summary>
        public List<string> RequestValidationErrors { get; set; } = new List<string>();

        /// <summary>
        /// The location header Uri for the new resource
        /// </summary>
        public Uri ResourceUri { get; set; }

        /// <summary>
        /// A collection of headers to add to the outgoing response
        /// </summary>
        public IHeaderDictionary Headers { get; set; } 

        /// <summary>
        /// Handle the response as instructed
        /// </summary>
        /// <param name="source"></param>
        /// <returns>
        /// The source controllers <see cref="IActionResult"/> determined by the response from the <see cref="ResourcePostHandler"/> handler
        /// </returns>
        public IActionResult Handle(ControllerBase source)
        {
            switch (StatusCode)
            {
               
                case HttpStatusCode.Created:
                    foreach (var item in Headers)
                    {
                        if (!source.Response.Headers.ContainsKey(item.Key)) { 
                            source.Response.Headers.Add(item.Key, item.Value);
                        }
                    }
                    return source.Created(ResourceUri, Model);

                case HttpStatusCode.BadRequest:
                   
                    return source.BadRequest(new RequestExceptionModel(RequestValidationErrors));
                
                default:
                    RequestValidationErrors.Add($"Unhandled condition set in {nameof(ResourcePostHandler)} for {nameof(ResourcePostResponse)}");
                    return source.StatusCode(500, new RequestExceptionModel(RequestValidationErrors));
            }
        }
    }
}
