using FluentValidation;
using SimpleLibrary.API.Requests.Authors;

using static SimpleLibrary.API.Validators.Shared.DateValidator;
using static SimpleLibrary.API.Validators.Shared.TagsValidator;

namespace SimpleLibrary.API.Validators.Authors;

public class PostAuthorRequestValidator : AbstractValidator<PostAuthorRequest>
{
    public PostAuthorRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Author first name is required.")
            .MaximumLength(100).WithMessage("Author first name cannot exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Author last name is required.")
            .MaximumLength(100).WithMessage("Author last name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Author description cannot exceed 1000 characters.");

        RuleFor(x => x.BornDate)
            .Must(BeAValidDate).When(x => !string.IsNullOrEmpty(x.BornDate))
            .WithMessage("Invalid BorndDate format. Please use the following format: YYYY-MM-DD")
            .Must(BeADateInThePast).When(x => !string.IsNullOrWhiteSpace(x.BornDate))
            .WithMessage("Born date must be a valid date in the past.");

        RuleFor(x => x.Tags)
            .Must(BeAValidTagFormat).When(x => x.Tags != null)
            .WithMessage("Invalid tags format. Please do not use commas in tags.");
    }
}
