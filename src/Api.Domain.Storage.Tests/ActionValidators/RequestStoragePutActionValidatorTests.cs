using Api.Domain.Storage.Delete;
using Api.Domain.Storage.Put;

namespace Api.Domain.Storage.Tests.ActionValidators
{
    [TestFixture]
    public class RequestStoragePutActionValidatorTests :
        ResourceStorageActionValidatorTestBase<ResourceStoragePutActionValidator, ResourceStoragePutRequest, ResourceStoragePutResponse>
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
                                        bool makeSureResourceExists,
                                        bool rejectIfHasChangedByDate,
                                        bool rejectIfHasChangedByETag,
                                        HttpStatusCodes statusCodeToExpect,
                                        int errorCountToExepct,
                                        bool modelAvailableAfterValidation
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
            var rejectifChangedFromEtag = rejectIfHasChangedByETag ?
                                new List<string>() { Guid.NewGuid().ToString() }
                                : new List<string>();

            var rejectIfChangedSinceDateTime = rejectIfHasChangedByDate ?
                                        Model.Modified.Value.AddMinutes(-10)
                                        : DateTimeOffset.MaxValue;

            Request = new ResourceStoragePutRequest()
            {
                Id = Model?.Id ?? Guid.NewGuid(),
                ETags = rejectifChangedFromEtag,
                UnmodifiedSince = rejectIfChangedSinceDateTime,
                Namespace = "my",
                OwnerId = Guid.NewGuid(),
                RequestId = Guid.NewGuid()

            };

            if (!makeSureResourceExists)
            {
                Model = null;
            }

            (Model, Response) = base.Act();

            base.AssertTheResultsAreAsExpected(statusCodeToExpect, errorCountToExepct, modelAvailableAfterValidation);
        }
    }
}
