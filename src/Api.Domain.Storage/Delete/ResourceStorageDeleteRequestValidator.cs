using FluentValidation;

namespace Api.Domain.Storage.Delete
{
    public class ResourceStorageDeleteRequestValidator : AbstractValidator<ResourceStorageDeleteRequest>
    {
        public ResourceStorageDeleteRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Namespace).NotEmpty();
            RuleFor(x => x.Namespace).Matches(@"^(?:(?:((?![0-9_])[a-zA-Z0-9_]+)[\.\/\\]?)+)(?<!\.)");
            RuleFor(x => x.OwnerId).NotEmpty();
            RuleFor(x => x.RequestId).NotEmpty();
        }
    }
}
