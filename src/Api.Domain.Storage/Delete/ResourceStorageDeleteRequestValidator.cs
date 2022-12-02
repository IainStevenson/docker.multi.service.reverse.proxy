using FluentValidation;

namespace Api.Domain.Storage.Delete
{
    public class ResourceStorageDeleteRequestValidator : AbstractValidator<ResourceStorageDeleteRequest>
    {
        public ResourceStorageDeleteRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.ContentNamespace).NotEmpty();
            RuleFor(x => x.ContentNamespace).Matches(@"^(.+\..+)|(.+\/.+)$");
            RuleFor(x => x.OwnerId).NotEmpty();
            RuleFor(x => x.RequestId).NotEmpty();
        }
    }
}
