using FluentValidation;

namespace Api.Domain.Storage.Get
{
    public class ResourceStorageGetOneRequestValidator : AbstractValidator<ResourceStorageGetOneRequest>
    {
        public ResourceStorageGetOneRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.OwnerId).NotEmpty();
            RuleFor(x => x.RequestId).NotEmpty();

            RuleFor(r=>r.IfModifiedSince).InclusiveBetween(DateTimeOffset.MinValue, DateTimeOffset.UtcNow);

            RuleFor(x => x.Namespace).NotNull();
            RuleFor(x => x.Namespace).NotEmpty(); // Needed?
            RuleFor(x => x.Namespace).Matches("^(?:(?:((?![0-9_])[a-zA-Z0-9_]+)\\.?)+)(?<!\\.)$");
            
            

        }
    }

}
