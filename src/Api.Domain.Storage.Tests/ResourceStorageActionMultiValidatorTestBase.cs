using Data.Model.Storage;
using MediatR;

namespace Api.Domain.Storage.Tests
{
    [TestFixture]
    public class ResourceStorageActionMultiValidatorTestBase<TValidator, TRequest, TResponse>
      where TValidator : IResourceStorageActionMultiValidator<TRequest, TResponse>
      where TRequest : IRequest<TResponse>
      where TResponse : ResourceStorageResponseBase
    {
        protected TValidator Unit;
        protected TRequest Request;
        protected TResponse Response;
        protected List<Resource> Models = new List<Resource>();

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

        protected (List<Resource>, TResponse) Act()
        {
            return ((List<Resource>, TResponse))Unit.Validate(Models, Request, Response);
        }

        /// <summary>
        /// Perform the required assertions
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="errorCount"></param>
        /// <param name="resourceIsProduced"></param>

        protected void AssertTheResultsAreAsExpected(HttpStatusCodes statusCode, int errorCount, int expectedItemCount)
        {
            Assert.That(Response.StatusCode, Is.EqualTo(statusCode));
            Assert.That(Response.RequestValidationErrors, Has.Count.EqualTo(errorCount));
            Assert.That(Models, Has.Count.EqualTo(expectedItemCount));
        }
    }
}
