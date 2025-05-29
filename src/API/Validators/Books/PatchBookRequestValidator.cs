using FluentValidation;
using SimpleLibrary.API.Requests.Books;
using SimpleLibrary.API.Validators.Shared;
using SimpleLibrary.Domain.Enumerations;

namespace SimpleLibrary.API.Validators.Books;

public class PatchBookRequestValidator : AbstractValidator<PatchBookRequest>
{
    public PatchBookRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Book ID is required.")
            .Must(GuidValidator.BeAValidGuid).WithMessage("Invalid Book ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");

        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.ReleaseDate)
            .Must(DateValidator.BeAValidDate).When(x => !string.IsNullOrWhiteSpace(x.ReleaseDate))
            .WithMessage("Invalid date format for ReleaseDate. Please use the following format: YYYY-MM-DD");

        RuleFor(x => x.Language)
            .IsEnumName(typeof(Language)).When(x => !string.IsNullOrWhiteSpace(x.Language))
            .WithMessage($"Invalid Language value. Available values: {Enum.GetNames(typeof(Language))}");

        RuleFor(x => x.Tags)
            .Must(TagsValidator.BeAValidTagFormat).When(x => x.Tags != null)
            .WithMessage("Invalid tags format. Please do not use commas in tags.");

        RuleFor(x => x.AuthorId)
            .Must(GuidValidator.BeAValidNullableGuid).When(x => !string.IsNullOrWhiteSpace(x.AuthorId))
            .WithMessage("Invalid Author ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");

        RuleFor(x => x.CategoryId)
            .Must(GuidValidator.BeAValidNullableGuid).When(x => !string.IsNullOrWhiteSpace(x.CategoryId))
            .WithMessage("Invalid Category ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
    }
}