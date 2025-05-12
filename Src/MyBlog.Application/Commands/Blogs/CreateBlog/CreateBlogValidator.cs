using FluentValidation;

namespace MyBlog.Application.Commands.Blogs.CreateBlog;

public class CreateBlogValidator : AbstractValidator<CreateBlogCommand>
{
    public CreateBlogValidator()
    {
        RuleFor(b => b.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage(
                $"{nameof(CreateBlogCommand.Title)} can't be empty and only allow 200 characters"
            );
        RuleFor(b => b.Content)
            .NotEmpty()
            .WithMessage($"{nameof(CreateBlogCommand.Content)} can't be empty");
        RuleFor(b => b.PublishDate).GreaterThanOrEqualTo(DateTime.UtcNow);
    }
}
