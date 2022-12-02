using Data.Model.Storage;
using MediatR;
using MongoDB.Bson;

namespace Api.Domain.Storage.Tests
{

    [TestFixture]
    public class ResourceStorageActionValidatorTestBase<TValidator, TRequest, TResponse>
        where TValidator : IResourceStorageActionValidator<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : ResourceStorageResponseBase
    {
        protected TValidator Unit;
        protected TRequest Request;
        protected TResponse Response;
        protected Resource? Model;

        [SetUp]
        public void Setup()
        {
            Unit = Activator.CreateInstance<TValidator>();
            Request = Activator.CreateInstance<TRequest>();
            Response = Activator.CreateInstance<TResponse>();
        }

        /// <summary>
        ///  Perform the test action
        /// </summary>
        /// <returns></returns>

        protected (Resource?, TResponse) Act()
        {
            return Unit.Validate(Model, Request, Response);
        }

        /// <summary>
        /// Perform the required assertions
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="errorCount"></param>
        /// <param name="resourceIsProduced"></param>

        protected void AssertTheResultsAreAsExpected(ApiDomainStatusCodes statusCode, int errorCount, bool modelAvailableAfterValidation)
        {
            if (modelAvailableAfterValidation)
            {
                Assert.That(Model, Is.Not.Null);
            }
            else
            {
                Assert.That(Model, Is.Null);
            }

            Assert.That(Response.StatusCode, Is.EqualTo(statusCode));
            Assert.That(Response.RequestValidationErrors.Count, Is.EqualTo(errorCount));
        }

    }
}
