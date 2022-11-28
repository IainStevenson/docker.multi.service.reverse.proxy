using Api.Domain.Storage.Get;

namespace Api.Domain.Storage.Tests.ActionValidators
{
    [TestFixture]
    public class RequestStorageGetManyActionValidatorTests :
     ResourceStorageActionMultiValidatorTestBase<ResourceStorageGetManyActionValidator, ResourceStorageGetManyRequest, ResourceStorageGetManyResponse>
    { 
    
    }
}
