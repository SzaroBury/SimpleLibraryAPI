using FluentValidation;
using SimpleLibrary.API.Requests.Categories;
using SimpleLibrary.API.Validators.Shared;

namespace SimpleLibrary.API.Validators.Categories;

public class PatchCategoryRequestValidator : AbstractValidator<PatchCategoryRequest>
{
    public PatchCategoryRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID is required.")
            .Must(GuidValidator.BeAValidGuid).WithMessage("Invalid Category ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleForEach(x => x.Tags)
            .NotEmpty().WithMessage("Tags cannot be empty.")
            .MaximumLength(50).WithMessage("Each tag cannot exceed 50 characters.");

        RuleFor(x => x.ParentCategoryId)
            .Must(GuidValidator.BeAValidNullableGuid).WithMessage("Invalid ParentCategory ID format. Please send the ID in the following format: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX, where each X is a hexadecimal digit (0-9 or A-F). Example: 123e4567-e89b-12d3-a456-426614174000.");
    }
}