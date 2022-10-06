using FluentValidation;
using System.Net;

namespace Api.Domain.Handling.Resource.Put
{
    public class ResourceResponsePutRequestValidator : AbstractValidator<ResourceResponsePutRequest>
    {
        private List<HttpStatusCode> _acceptableCodes = new List<HttpStatusCode>() {
        HttpStatusCode.PreconditionFailed, HttpStatusCode.OK, HttpStatusCode.NotFound
        };
        public ResourceResponsePutRequestValidator()
        {
            RuleFor(x => x.Scheme).NotNull();
            RuleFor(x => x.Host).NotNull();
            RuleFor(x => x.PathBase).NotNull();
            RuleFor(x => x.Path).NotNull();
            RuleFor(rule => rule.StatusCode).Must(value => _acceptableCodes.Contains(value));
        }
    }
}
