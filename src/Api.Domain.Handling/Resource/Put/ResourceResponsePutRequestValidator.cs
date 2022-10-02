using FluentValidation;

namespace Api.Domain.Handling.Resource.Put
{
    public class ResourceResponsePutRequestValidator : AbstractValidator<ResourceResponsePutRequest>
    {
        public ResourceResponsePutRequestValidator()
        {
            RuleFor(x => x.Scheme).NotNull();
            RuleFor(x => x.Host).NotNull();
            RuleFor(x => x.PathBase).NotNull();
            RuleFor(x => x.Path).NotNull();
        }
    }
}
