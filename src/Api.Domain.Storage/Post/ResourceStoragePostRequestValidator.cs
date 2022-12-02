using FluentValidation;

namespace Api.Domain.Storage.Post
{
    public class ResourceStoragePostRequestValidator : AbstractValidator<ResourceStoragePostRequest>
    {
        public ResourceStoragePostRequestValidator()
        {
            RuleFor(x => x.Content).NotNull();
            RuleFor(x => x.ContentNamespace).NotEmpty(); 
            RuleFor(x => x.ContentNamespace).Matches(@"^(.+\..+)|(.+\/.+)$");
            RuleFor(x => x.OwnerId).NotEmpty();
            RuleFor(x => x.RequestId).NotEmpty();
        }
    }
}
