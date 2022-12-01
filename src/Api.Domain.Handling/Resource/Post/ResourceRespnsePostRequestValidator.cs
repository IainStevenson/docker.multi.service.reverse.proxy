using FluentValidation;
using System.Net;

namespace Api.Domain.Handling.Resource.Post
{
    public class ResourceRespnsePostRequestValidator : AbstractValidator<ResourceResponsePostRequest>
    {
        private List<HttpStatusCode> _acceptableCodes = new List<HttpStatusCode>() { HttpStatusCode.BadRequest, HttpStatusCode.Created };

        public ResourceRespnsePostRequestValidator()
        {
            RuleFor(rule => rule.Model).NotNull();
            RuleFor(rule => rule.Scheme).NotEmpty();
            RuleFor(rule => rule.Host).NotEmpty();
            RuleFor(rule => rule.PathBase).NotEmpty();
            RuleFor(rule => rule.Path).NotEmpty();
            RuleFor(rules => rules.Namespace).NotEmpty();   
            RuleFor(rule => rule.StatusCode).Must(value => _acceptableCodes.Contains(value));
        }
    }
}
