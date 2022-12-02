using Api.Domain.Storage.Put;
using FluentValidation;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetManyRequestValidator : AbstractValidator<ResourceStorageGetManyRequest>
    {
        public ResourceStorageGetManyRequestValidator()
        {
            RuleFor(r => r.IfModifiedSince).InclusiveBetween(DateTimeOffset.MinValue, DateTimeOffset.UtcNow);

            RuleFor(x => x.ContentNamespace).NotEmpty();
            RuleFor(x => x.ContentNamespace).Matches(@"^(.+\..+)|(.+\/.+)$");
        }
    }

}
