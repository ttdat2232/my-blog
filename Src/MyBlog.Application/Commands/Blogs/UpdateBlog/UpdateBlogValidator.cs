using FluentValidation;

namespace MyBlog.Application.Commands.Blogs.UpdateBlog;

public class UpdateBlogValidator : AbstractValidator<UpdateBlogCommand>
{
    public UpdateBlogValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Title cannot be empty and must be less than 200 characters");
        RuleFor(x => x.Content).NotEmpty().WithMessage("Content cannot be empty");
        RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid blog status");
    }
}
