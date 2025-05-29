using FluentValidation;
using SimpleLibrary.API.Requests.Borrowings;
using SimpleLibrary.API.Validators.Shared;

namespace SimpleLibrary.API.Validators.Borrowings;

public class PatchBorrowingRequestValidator : AbstractValidator<PatchBorrowingRequest>
{
    public PatchBorrowingRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Borrowing ID is required.")
            .Must(GuidValidator.BeAValidGuid).WithMessage("Invalid Borrowing ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");

        RuleFor(x => x.StartedDate)
            .Must(DateValidator.BeAValidDate).When(x => !string.IsNullOrEmpty(x.StartedDate))
            .WithMessage("Invalid date format for Started Date.");

        RuleFor(x => x.ActualReturnDate)
            .Must(DateValidator.BeAValidDate).When(x => !string.IsNullOrEmpty(x.ActualReturnDate))
            .WithMessage("Invalid date format for Actual Return Date.");

        RuleFor(x => x.CopyId)
            .Must(GuidValidator.BeAValidNullableGuid).When(x => x.CopyId != null)
            .WithMessage("Invalid Copy ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");

        RuleFor(x => x.ReaderId)
            .Must(GuidValidator.BeAValidNullableGuid).When(x => x.ReaderId != null)
            .WithMessage("Invalid Reader ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
    }
}