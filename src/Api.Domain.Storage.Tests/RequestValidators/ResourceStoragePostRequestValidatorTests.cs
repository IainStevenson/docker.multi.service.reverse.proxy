using Api.Domain.Storage.Get;
using Api.Domain.Storage.Post;

namespace Api.Domain.Storage.Tests.RequestValidators
{

    public class ResourceStoragePostRequestValidatorTests :
            ResourceStorageRequstValidatorTestBase<ResourceStoragePostRequestValidator, ResourceStoragePostRequest>
    {
        [Test]
        [TestCase("01000000-0000-0000-0000-000000000000", "01000000-0000-0000-0000-000000000000", "my.namespace", "{\"id\": 1, \"data\":\"1234\"}", "id", true, "")]
        [TestCase("00000000-0000-0000-0000-000000000000", "01000000-0000-0000-0000-000000000000", "my.namespace", "{\"id\": 1, \"data\":\"1234\"}", "id", false, "OwnerId")]
        [TestCase("01000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000", "my.namespace", "{\"id\": 1, \"data\":\"1234\"}", "id", false, "RequestId")]
        [TestCase("01000000-0000-0000-0000-000000000000", "01000000-0000-0000-0000-000000000000", "my~namespace", "{\"id\": 1, \"data\":\"1234\"}", "id", true, "")]
        [TestCase("01000000-0000-0000-0000-000000000000", "01000000-0000-0000-0000-000000000000", "my\\namespace", null, "id", false, "Content")]
        [TestCase("01000000-0000-0000-0000-000000000000", "01000000-0000-0000-0000-000000000000", "my/namespace", "{\"id\": 1, \"data\":\"1234\"}", "", true, "")]
        public void TestScenario(
            string OwnerId,
            string RequestId,
            string Namespace,
            string content,
            string keys,
            bool isValid,
            string expectedErrorProperties
           )
        {
            // Arrange
            Request = new ResourceStoragePostRequest()
            {
                OwnerId = new Guid(OwnerId),
                RequestId = new Guid(RequestId),
                Namespace = Namespace,
                Content = content,
                Keys = keys,
            };

            // Act          
            var result = base.Act();
            // Assert
            base.AssertTheResultsAreAsExpected(result, isValid, expectedErrorProperties);

        }
    }
}
