using Api.Domain.Storage.Get;
using Api.Domain.Storage.Put;

namespace Api.Domain.Storage.Tests.ActionValidators
{
    [TestFixture]
    public class RequestStorageGetOneActionValidatorTests :
       ResourceStorageActionValidatorTestBase<ResourceStorageGetOneActionValidator, ResourceStorageGetOneRequest, ResourceStorageGetOneResponse>
    {

        [TestCase(false, false, false, ApiDomainStatusCodes.NOTFOUND, 1, false)]
        [TestCase(false, false, true, ApiDomainStatusCodes.NOTFOUND, 1, false)]
        [TestCase(false, true, false, ApiDomainStatusCodes.NOTFOUND, 1, false)]
        [TestCase(false, true, true, ApiDomainStatusCodes.NOTFOUND, 1, false)]
        [TestCase(true, true, false, ApiDomainStatusCodes.NOTMODIFIED, 1, false)]
        [TestCase(true, false, true, ApiDomainStatusCodes.NOTMODIFIED, 1, false)]
        [TestCase(true, true, true, ApiDomainStatusCodes.NOTMODIFIED, 1, false)]
        [TestCase(true, false, false, ApiDomainStatusCodes.OK, 0, true)]
        public void TestScenario(
                                           bool makeSureResourceExists,
                                           bool ignoreUnlessHasChangedByDate,
                                           bool ignoreUnlessHasChangedByETag,
                                           ApiDomainStatusCodes statusCodeToExpect,
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
            var ignoreUnlessChangedFromEtag = ignoreUnlessHasChangedByETag ?
                                new List<string>() { Model.Etag }
                                : 
                                new List<string>();

            var ignoreUnlessChangedSinceDateTime = ignoreUnlessHasChangedByDate ?
                                        Model.Modified.Value.AddMinutes(10)
                                        :
                                        DateTimeOffset.MinValue;


            Request = new ResourceStorageGetOneRequest()
            {
                Id = Model?.Id ?? Guid.NewGuid(),
                IfNotETags = ignoreUnlessChangedFromEtag,
                IfModifiedSince = ignoreUnlessChangedSinceDateTime,
                ContentNamespace = "my",
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
