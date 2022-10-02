using FluentValidation;
using System.Net;

namespace Api.Domain.Handling.Resource.Delete
{
    public class ResourceResponseDeleteRequestValidator : AbstractValidator<ResourceResponseDeleteRequest> {

        private List<HttpStatusCode> _acceptableCodes = new List<HttpStatusCode>() {
        HttpStatusCode.NotFound,  HttpStatusCode.NoContent, HttpStatusCode.Gone, HttpStatusCode.PreconditionFailed};

        public ResourceResponseDeleteRequestValidator()
        {
            RuleFor(rule => rule.StatusCode).Must(value => _acceptableCodes.Contains(value));
        }
    }


}
