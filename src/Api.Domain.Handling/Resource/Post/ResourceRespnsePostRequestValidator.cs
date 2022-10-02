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
            RuleFor(rule => rule.Scheme).NotNull();
            RuleFor(rule => rule.Host).NotNull();
            RuleFor(rule => rule.PathBase).NotNull();
            RuleFor(rule => rule.Path).NotNull();
            RuleFor(x => x.Namespace).NotNull(); 
            RuleFor(x => x.Namespace).Matches("^(?:(?:((?![0-9_])[a-zA-Z0-9_]+)\\.?)+)(?<!\\.)$");
            RuleFor(rule => rule.StatusCode).Must(value => _acceptableCodes.Contains(value));
        }
    }
}
