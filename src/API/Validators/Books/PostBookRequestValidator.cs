using FluentValidation;
using SimpleLibrary.API.Requests.Books;
using SimpleLibrary.API.Validators.Shared;
using SimpleLibrary.Domain.Enumerations;

namespace SimpleLibrary.API.Validators.Books;

public class PostBookRequestValidator : AbstractValidator<PostBookRequest>
{
    public PostBookRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.ReleaseDate)
            .NotEmpty().WithMessage("Release date is required.")
            .Must(DateValidator.BeAValidDate).WithMessage("Invalid date format for Release Date.");

        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required.")
            .IsEnumName(typeof(Language))
            .WithMessage($"Invalid Language value. Available values: {Enum.GetNames(typeof(Language))}");

        RuleFor(x => x.Tags)
            .NotEmpty().WithMessage("Tags are required.")
            .Must(TagsValidator.BeAValidTagFormat).WithMessage("Invalid tags format. Please do not use commas in tags.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.")
            .Must(GuidValidator.BeAValidGuid).WithMessage("Invalid Category ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("Author ID is required.")
            .Must(GuidValidator.BeAValidGuid).WithMessage("Invalid Author ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
    }
}