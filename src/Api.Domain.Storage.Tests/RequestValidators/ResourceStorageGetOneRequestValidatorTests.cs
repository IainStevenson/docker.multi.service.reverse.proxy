using Api.Domain.Storage.Get;

namespace Api.Domain.Storage.Tests.RequestValidators
{
    [TestFixture]
    public class ResourceStorageGetOneRequestValidatorTests: 
        ResourceStorageRequstValidatorTestBase<ResourceStorageGetOneRequestValidator, ResourceStorageGetOneRequest>
    {
       

        [Test]
        [TestCase("01000000-0000-0000-0000-000000000000", "01000000-0000-0000-0000-000000000000", "01000000-0000-0000-0000-000000000000", "my.namespace", "01000000-0000-0000-0000-000000000000", null, true, "")]
        [TestCase("00000000-0000-0000-0000-000000000000", "01000000-0000-0000-0000-000000000000", "01000000-0000-0000-0000-000000000000", "my.namespace", "01000000-0000-0000-0000-000000000000", null, false, "Id")]
        [TestCase("01000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000", "01000000-0000-0000-0000-000000000000", "my.namespace", "01000000-0000-0000-0000-000000000000", null, false, "OwnerId")]
        [TestCase("01000000-0000-0000-0000-000000000000", "01000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000", "my.namespace", "01000000-0000-0000-0000-000000000000", null, false, "RequestId")]
        [TestCase("01000000-0000-0000-0000-000000000000", "01000000-0000-0000-0000-000000000000", "01000000-0000-0000-0000-000000000000", "my/namespace", "01000000-0000-0000-0000-000000000000", null, false, "Namespace")]

        public void TestScenario(
             string Id,
             string OwnerId,
             string RequestId,
             string Namespace,
             string IfNotETags,
             string IfModifiedSince,
             bool isValid,
             string expectedErrorProperties

            )
        {
            Request = new ResourceStorageGetOneRequest()
            {
                Id = new Guid(Id),
                OwnerId = new Guid(OwnerId),
                RequestId = new Guid(RequestId),
                Namespace = Namespace,
                IfNotETags = IfNotETags == null ? new List<string>() : IfNotETags.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                IfModifiedSince = IfModifiedSince == null ? DateTimeOffset.MinValue : DateTimeOffset.Parse(IfModifiedSince)
            };

            // Act          
            var result = base.Act();
            // Assert
            base.AssertTheResultsAreAsExpected(result, isValid, expectedErrorProperties);

        }
    }
}
