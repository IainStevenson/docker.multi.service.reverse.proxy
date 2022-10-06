using FluentValidation;

namespace Api.Domain.Storage.Put
{
    public class ResourceStoragePutRequestValidator : AbstractValidator<ResourceStoragePutRequest>
    {
        public ResourceStoragePutRequestValidator()
        {
            RuleFor(x => x.Content).NotNull();
            RuleFor(x => x.Namespace).NotNull();
            RuleFor(x => x.Namespace).Matches("^(?:(?:((?![0-9_])[a-zA-Z0-9_]+)\\.?)+)(?<!\\.)$");
            RuleFor(x => x.OwnerId).NotEmpty();
            RuleFor(x => x.RequestId).NotEmpty();
        }
    }

}
