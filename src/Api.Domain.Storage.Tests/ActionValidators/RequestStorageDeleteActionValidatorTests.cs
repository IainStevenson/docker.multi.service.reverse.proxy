using Api.Domain.Storage.Delete;

namespace Api.Domain.Storage.Tests.ActionValidators
{

    [TestFixture]
    public class RequestStorageDeleteActionValidatorTests :
        ResourceStorageActionValidatorTestBase<ResourceStorageDeleteActionValidator, ResourceStorageDeleteRequest, ResourceStorageDeleteResponse>
    {

        [TestCase(false, false, false, HttpStatusCodes.NOTFOUND, 1, false)]
        [TestCase(false, false, true, HttpStatusCodes.NOTFOUND, 1, false)]
        [TestCase(false, true, false, HttpStatusCodes.NOTFOUND, 1, false)]
        [TestCase(false, true, true, HttpStatusCodes.NOTFOUND, 1, false)]
        [TestCase(true, true, false, HttpStatusCodes.PRECONDITIONFAILED, 1, false)]
        [TestCase(true, false, true, HttpStatusCodes.PRECONDITIONFAILED, 1, false)]
        [TestCase(true, true, true, HttpStatusCodes.PRECONDITIONFAILED, 1, false)]
        [TestCase(true, false, false, HttpStatusCodes.OK, 0, true)]
        public void TestScenario(
                                    bool resourceExists, 
                                    bool IfUnchangedSince, 
                                    bool ifNotETags, 
                                    HttpStatusCodes statusCodeToExpect, 
                                    int errorCountToExepct,
                                    bool resourceIsProduced
            )
        {
            // Arrange
            Model = new Data.Model.Storage.Resource()
            {
                Created = DateTimeOffset.UtcNow,
                Modified = DateTimeOffset.UtcNow,
                Content = new { },
                Etag = Guid.NewGuid().ToString(),
                Namespace = "my.namespace",
                OwnerId = Guid.NewGuid()
            };
            var ifNotEtagsList = ifNotETags ?
                                new List<string>() { Guid.NewGuid().ToString() }
                                : new List<string>();

            var isUnchangedSinceDateTime = IfUnchangedSince ?
                                        Model.Modified.Value.AddMinutes(-10)
                                        : DateTimeOffset.MaxValue;

            Request = new ResourceStorageDeleteRequest()
            {
                Id = Model?.Id ?? Guid.NewGuid(),
                IsNotETags = ifNotEtagsList,
                IsUnchangedSince = isUnchangedSinceDateTime,
                Namespace = "my",
                OwnerId = Guid.NewGuid(),
                RequestId = Guid.NewGuid()

            };

            if (!resourceExists)
            {
                Model = null;
            }

            (Model, Response) =  base.Act();

            base.AssertTheResultsAreAsExpected(statusCodeToExpect, errorCountToExepct, resourceIsProduced);
        }

    }
}
