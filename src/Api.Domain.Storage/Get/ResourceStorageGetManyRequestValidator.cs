using FluentValidation;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetManyRequestValidator : AbstractValidator<ResourceStorageGetManyRequest>
    {
        public ResourceStorageGetManyRequestValidator()
        {
            RuleFor(r => r.IfModifiedSince).InclusiveBetween(DateTimeOffset.MinValue, DateTimeOffset.UtcNow);

            RuleFor(x => x.Namespace).NotEmpty(); // Needed?
            RuleFor(x => x.Namespace).Matches("^(?:(?:((?![0-9_])[a-zA-Z0-9_]+)\\.?)+)(?<!\\.)$");
        }
    }

}
