using FluentValidation;
using SimpleLibrary.API.Requests.Categories;
using SimpleLibrary.API.Validators.Shared;

namespace SimpleLibrary.API.Validators.Categories;

public class PostCategoryRequestValidator : AbstractValidator<PostCategoryRequest>
{
    public PostCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleForEach(x => x.Tags)
            .MaximumLength(50).WithMessage("Each tag cannot exceed 50 characters.");

        RuleFor(x => x.Tags)
            .Must(TagsValidator.BeAValidTagFormat).WithMessage("");

        RuleFor(x => x.ParentCategoryId)
            .Must(GuidValidator.BeAValidNullableGuid).WithMessage("Invalid Parent Category ID format.");
    }
}