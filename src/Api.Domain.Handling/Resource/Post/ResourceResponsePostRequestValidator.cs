using FluentValidation;
using System.Net;

namespace Api.Domain.Handling.Resource.Post
{
    public class ResourceResponsePostRequestValidator : AbstractValidator<ResourceResponsePostRequest>
    {
        private List<HttpStatusCode> _acceptableCodes = new List<HttpStatusCode>() { HttpStatusCode.BadRequest, HttpStatusCode.Created };

        public ResourceResponsePostRequestValidator()
        {
            RuleFor(rule => rule.Model).NotNull();
            RuleFor(rule => rule.Scheme).NotEmpty();
            RuleFor(rule => rule.Host).NotEmpty();
            RuleFor(rule => rule.PathBase).NotEmpty();
            RuleFor(rule => rule.Path).NotEmpty();
            RuleFor(rules => rules.ContentNamespace).NotEmpty();   
            RuleFor(rule => rule.StatusCode).Must(value => _acceptableCodes.Contains(value));
        }
    }
}
