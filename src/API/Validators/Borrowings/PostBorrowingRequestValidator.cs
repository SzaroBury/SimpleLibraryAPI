using FluentValidation;
using SimpleLibrary.API.Requests.Borrowings;
using SimpleLibrary.API.Validators.Shared;

namespace SimpleLibrary.API.Validators.Borrowings;

public class PostBorrowingRequestValidator : AbstractValidator<PostBorrowingRequest>
{
    public PostBorrowingRequestValidator()
    {
        RuleFor(x => x.CopyId)
            .NotEmpty().WithMessage("Copy ID is required.")
            .Must(GuidValidator.BeAValidGuid).WithMessage("Invalid Copy ID format.");

        RuleFor(x => x.ReaderId)
            .NotEmpty().WithMessage("Reader ID is required.")
            .Must(GuidValidator.BeAValidGuid).WithMessage("Invalid Reader ID format.");

        RuleFor(x => x.StartedDate)
            .Must(DateValidator.BeAValidDate).When(x => !string.IsNullOrWhiteSpace(x.StartedDate))
            .WithMessage("Invalid date format for Started Date.");

        RuleFor(x => x.ActualReturnDate)
            .Must(DateValidator.BeAValidDate).When(x => !string.IsNullOrWhiteSpace(x.ActualReturnDate))
            .WithMessage("Invalid date format for Actual Return Date.");
    }
}