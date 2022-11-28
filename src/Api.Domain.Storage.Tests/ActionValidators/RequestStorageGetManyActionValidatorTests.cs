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
        [TestCase(false, 0, HttpStatusCodes.NOTFOUND, 1, 0)]
        [TestCase(false, 1, HttpStatusCodes.NOTFOUND, 1, 0)]
        [TestCase(false, 2, HttpStatusCodes.NOTFOUND, 1, 0)]
        [TestCase(true, 0, HttpStatusCodes.OK, 0, 2)]
        [TestCase(true, 1, HttpStatusCodes.OK, 0, 1)]
        [TestCase(true, 2, HttpStatusCodes.NOTMODIFIED, 1, 0)]
        [TestCase(true, 0, HttpStatusCodes.OK, 0, 2)]
        public void TestScenario(
                                bool provideExistingItems,
                                int applyChangedSinceTimestamp,
                                HttpStatusCodes statusCodeToExpect,
                                int errorCountToExpect,
                                int numberOfItemsExpected
                      )
        {
            // Arrange
            Models = new List<Data.Model.Storage.Resource>() {
                    new Data.Model.Storage.Resource()
                    {
                        Created = DateTimeOffset.UtcNow.AddMinutes(-10),
                        Modified = DateTimeOffset.UtcNow.AddMinutes(-10),
                        Content = new { },
                        Etag = Guid.NewGuid().ToString(),
                        Namespace = "my.namespace",
                        OwnerId = Guid.NewGuid()
                    },
                    new Data.Model.Storage.Resource()
                    {
                        Created = DateTimeOffset.UtcNow.AddMinutes(-5),
                        Modified = DateTimeOffset.UtcNow.AddMinutes(-5),
                        Content = new { },
                        Etag = Guid.NewGuid().ToString(),
                        Namespace = "my.namespace",
                        OwnerId = Guid.NewGuid()
                    }};
           

            var applyChangeTimeStamps = new List<DateTimeOffset>()
            {
                DateTimeOffset.MinValue, // return two (all)
                DateTimeOffset.UtcNow.AddMinutes(-8), // return 2nd one
                DateTimeOffset.MaxValue // return none
            };




            Request = new ResourceStorageGetManyRequest()
            {               
                
                IfModifiedSince = applyChangeTimeStamps[applyChangedSinceTimestamp],
                Namespace = "my",
                OwnerId = Guid.NewGuid(),
                RequestId = Guid.NewGuid()

            };

            if (!provideExistingItems)
            {
                Models.Clear();
            }

            (Models, Response) = base.Act();

            base.AssertTheResultsAreAsExpected(statusCodeToExpect, errorCountToExpect, numberOfItemsExpected);
        }
    }
}
