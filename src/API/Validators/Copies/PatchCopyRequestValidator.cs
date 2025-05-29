using FluentValidation;
using SimpleLibrary.API.Requests.Copies;
using SimpleLibrary.API.Validators.Shared;
using SimpleLibrary.Domain.Enumerations;
using static SimpleLibrary.API.Validators.Shared.GuidValidator;
using static SimpleLibrary.API.Validators.Shared.DateValidator;

namespace SimpleLibrary.API.Validators.Categories;

public class PatchCopyRequestValidator : AbstractValidator<PatchCopyRequest>
{
    public PatchCopyRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Copy ID is required.")
            .Must(BeAValidGuid).WithMessage("Invalid Copy ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");

        RuleFor(x => x.BookId).Must(BeAValidNullableGuid).When(x => !string.IsNullOrWhiteSpace(x.BookId))
        .WithMessage("");

        RuleFor(x => x.Shelf).GreaterThan(0).WithMessage("The Shelf must be a positive number.");

        RuleFor(x => x.Condition).IsEnumName(typeof(CopyCondition)).When(x => !string.IsNullOrWhiteSpace(x.Condition))
            .WithMessage($"Invalid CopyCondition value. Available values: {Enum.GetNames(typeof(CopyCondition))}");

        RuleFor(x => x.AcquisitionDate).Must(BeAValidDate).When(x => !string.IsNullOrWhiteSpace(x.AcquisitionDate))
            .WithMessage("");

        RuleFor(x => x.LastInspectionDate).Must(BeAValidDate).When(x => !string.IsNullOrWhiteSpace(x.LastInspectionDate))
            .WithMessage("");

        RuleFor(x => x.CopyNumber).GreaterThan(0).WithMessage("The CopyNumber must be a positive number.");
    }
}