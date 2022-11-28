using Api.Domain.Storage.Delete;
using Api.Domain.Storage.Get;
using Api.Domain.Storage.Post;

namespace Api.Domain.Storage.Tests.RequestValidators
{

    [TestFixture]
    public class ResourceStorageDeleteRequestValidatorTests :
        ResourceStorageRequstValidatorTestBase<ResourceStorageDeleteRequestValidator, ResourceStorageDeleteRequest>
    {
        [Test]
        [TestCase(
            "01000000-0000-0000-0000-000000000000", 
            "01000000-0000-0000-0000-000000000000", 
            "01000000-0000-0000-0000-000000000000", 
            "my.namespace",
            "01000000-0000-0000-0000-000000000000",
            null, 
            true, "")] // is ok
        [TestCase(
            "00000000-0000-0000-0000-000000000000", 
            "01000000-0000-0000-0000-000000000000", 
            "01000000-0000-0000-0000-000000000000", 
            "my.namespace", 
            "01000000-0000-0000-0000-000000000000", 
            null, 
            false, "Id")] // has missing id
        [TestCase(
            "01000000-0000-0000-0000-000000000000", 
            "00000000-0000-0000-0000-000000000000", 
            "01000000-0000-0000-0000-000000000000", 
            "my.namespace", 
            "01000000-0000-0000-0000-000000000000", 
            null, 
            false, "OwnerId")]// has missing ownerid
        [TestCase(
            "01000000-0000-0000-0000-000000000000", 
            "01000000-0000-0000-0000-000000000000", 
            "00000000-0000-0000-0000-000000000000", 
            "my.namespace", 
            "01000000-0000-0000-0000-000000000000", 
            null, 
            false, "RequestId")] // has missing request id
        [TestCase(
            "01000000-0000-0000-0000-000000000000", 
            "01000000-0000-0000-0000-000000000000",
            "01000000-0000-0000-0000-000000000000",
            "my/namespace", 
            "01000000-0000-0000-0000-000000000000",
            null, 
            false, "Namespace")]// has bad namespace

        public void TestScenario(
            string Id,
            string OwnerId,
            string RequestId,
            string Namespace,
            string IsNotETags,
            string IsUnchangedSince,
            bool isValid,
            string expectedErrorProperties

           )
        {
            Request = new ResourceStorageDeleteRequest()
            {
                Id = new Guid(Id),
                OwnerId = new Guid(OwnerId),
                RequestId = new Guid(RequestId),
                Namespace = Namespace,
                IsNotETags = IsNotETags == null ? new List<string>() : IsNotETags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                IsUnchangedSince = IsUnchangedSince == null ? DateTimeOffset.MaxValue : DateTimeOffset.Parse(IsUnchangedSince)
            };

            // Act          
            var result = base.Act();
            // Assert
            base.AssertTheResultsAreAsExpected(result, isValid, expectedErrorProperties);

        }        
    }
}
