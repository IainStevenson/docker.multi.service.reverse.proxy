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
            RuleFor(x => x.Scheme).NotEmpty();
            RuleFor(x => x.Host).NotEmpty();
            RuleFor(x => x.PathBase).NotEmpty();
            RuleFor(x => x.Path).NotEmpty();
            RuleFor(x => x.ContentNamespace).NotEmpty();
            RuleFor(rule => rule.StatusCode).Must(value => _acceptableCodes.Contains(value));
        }
    }
}
