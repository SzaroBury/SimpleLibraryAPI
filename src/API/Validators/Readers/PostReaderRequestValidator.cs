using FluentValidation;
using SimpleLibrary.API.Requests.Readers;

namespace SimpleLibrary.API.Validators.Readers;

public class PostReaderRequestValidator : AbstractValidator<PostReaderRequest>
{
    public PostReaderRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}