using Api.Domain.Storage.Get;
using System.Reflection;

namespace Api.Domain.Storage.Tests.ActionValidators
{
    /// <summary>
    /// This test class is checking the working of the action validator, which has teh task of working out one of three outcomes;
    /// OK, and may or may not contain any items.
    /// An empty collection with the notmodified status indicating none of them have changed since the date time specified
    /// </summary>
    [TestFixture]
    public class RequestStorageGetManyActionValidatorTests :
     ResourceStorageActionMultiValidatorTestBase<ResourceStorageGetManyActionValidator, ResourceStorageGetManyRequest, ResourceStorageGetManyResponse>
    {
        [TestCase(false, 0, ApiDomainStatusCodes.NOTFOUND, 1, 0)]
        [TestCase(false, 1, ApiDomainStatusCodes.NOTFOUND, 1, 0)]
        [TestCase(false, 2, ApiDomainStatusCodes.NOTFOUND, 1, 0)]
        [TestCase(true, 0, ApiDomainStatusCodes.OK, 0, 2)]
        [TestCase(true, 1, ApiDomainStatusCodes.OK, 0, 1)]
        [TestCase(true, 2, ApiDomainStatusCodes.NOTMODIFIED, 1, 0)]
        public void TestScenario(
                                bool provideExistingItems,
                                int applyChangedSinceTimestamp,
                                ApiDomainStatusCodes statusCodeToExpect,
                                int errorCountToExpect,
                                int numberOfItemsExpected
                      )
        {

            var tenMinutesAgo = DateTimeOffset.UtcNow.AddMinutes(-10);
            var eigthMinutesAgo = DateTimeOffset.UtcNow.AddMinutes(-8);
            var fiveMinutesAgo = DateTimeOffset.UtcNow.AddMinutes(-5);


            // Arrange
            Models = new List<Data.Model.Storage.Resource>();
            if (provideExistingItems)
            {

                Models.Add(new Data.Model.Storage.Resource()
                {
                    Created = tenMinutesAgo,
                    Modified = tenMinutesAgo,

                });
                Models.Add(new Data.Model.Storage.Resource()
                {
                    Created = tenMinutesAgo,
                    Modified = fiveMinutesAgo,

                });
            }

            var applyChangeSinceTimeStamps = new List<DateTimeOffset>()
            {
                DateTimeOffset.MinValue,
                eigthMinutesAgo,
                DateTimeOffset.MaxValue
            };

            Request = new ResourceStorageGetManyRequest()
            {
                IfModifiedSince = applyChangeSinceTimeStamps[applyChangedSinceTimestamp],
            };


            (Models, Response) = base.Act();

            base.AssertTheResultsAreAsExpected(statusCodeToExpect, errorCountToExpect, numberOfItemsExpected);
        }
    }
}
