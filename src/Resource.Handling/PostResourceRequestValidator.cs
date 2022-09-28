using FluentValidation;

namespace Resource.Handling
{
    public class PostResourceRequestValidator : AbstractValidator<PostResourceRequest>
    {
        public PostResourceRequestValidator()
        {
            RuleFor(x => x.Content).NotNull();
            RuleFor(x => x.Namespace).NotNull()
                .Matches("^(?:(?:((?![0-9_])[a-zA-Z0-9_]+)\\.?)+)(?<!\\.)$")
                .MaximumLength(1024);
            
            
            RuleFor(x => x.OwnerId).NotEmpty();
            RuleFor(x => x.RequestId).NotEmpty();
        }
    }
}