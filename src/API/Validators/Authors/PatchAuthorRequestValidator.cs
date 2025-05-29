using FluentValidation;
using SimpleLibrary.API.Requests.Authors;

using static SimpleLibrary.API.Validators.Shared.DateValidator;
using static SimpleLibrary.API.Validators.Shared.TagsValidator;
using static SimpleLibrary.API.Validators.Shared.GuidValidator;

namespace SimpleLibrary.API.Validators.Authors;

public class PatchAuthorRequestValidator : AbstractValidator<PatchAuthorRequest>
{
    public PatchAuthorRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Author ID is required.")
            .Must(BeAValidGuid).WithMessage("Invalid Author ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");

        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.BornDate)
            .Must(BeAValidDate).When(x => !string.IsNullOrEmpty(x.BornDate))
                .WithMessage("Invalid BornDate format. Please use the following format: YYYY-MM-DD")
            .Must(BeADateInThePast).When(x => !string.IsNullOrWhiteSpace(x.BornDate))
                .WithMessage("BornDate must be a valid date in the past.");

        RuleFor(x => x.Tags)
            .Must(BeAValidTagFormat).When(x => x != null)
            .WithMessage("Invalid tags format. Please do not use commas in tags.");
    }
}