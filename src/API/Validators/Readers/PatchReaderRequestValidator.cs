using FluentValidation;
using SimpleLibrary.API.Requests.Readers;

namespace SimpleLibrary.API.Validators.Readers;

public class PatchReaderRequestValidator : AbstractValidator<PatchReaderRequest>
{
    public PatchReaderRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID is required.");

        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format.");
    }
}