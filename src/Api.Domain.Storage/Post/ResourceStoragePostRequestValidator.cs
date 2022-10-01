using FluentValidation;

namespace Api.Domain.Storage.Post
{
    public class ResourceStoragePostRequestValidator : AbstractValidator<ResourceStoragePostRequest>
    {
        public ResourceStoragePostRequestValidator()
        {
            RuleFor(x => x.Content).NotNull();
            RuleFor(x => x.Namespace).NotNull(); // Needed?
            RuleFor(x => x.Namespace).Matches("^(?:(?:((?![0-9_])[a-zA-Z0-9_]+)\\.?)+)(?<!\\.)$");
            RuleFor(x => x.OwnerId).NotEmpty();
            RuleFor(x => x.RequestId).NotEmpty();
            //RuleFor(x => x.Scheme).NotNull();
            //RuleFor(x => x.Host).NotNull();
            //RuleFor(x => x.Path).NotNull();
            //RuleFor(x => x.Scheme).NotNull();
        }
    }

}
