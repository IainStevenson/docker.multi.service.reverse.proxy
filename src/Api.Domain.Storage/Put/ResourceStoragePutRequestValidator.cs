using Api.Domain.Storage.Post;
using FluentValidation;

namespace Api.Domain.Storage.Put
{
    public class ResourceStoragePutRequestValidator : AbstractValidator<ResourceStoragePutRequest>
    {
        public ResourceStoragePutRequestValidator()
        {
            RuleFor(x => x.Content).NotNull();
            RuleFor(x => x.ContentNamespace).NotNull();
            RuleFor(x => x.ContentNamespace).Matches(@"^(.+\..+)|(.+\/.+)$");
            RuleFor(x => x.OwnerId).NotEmpty();
            RuleFor(x => x.RequestId).NotEmpty();
        }
    }
}
