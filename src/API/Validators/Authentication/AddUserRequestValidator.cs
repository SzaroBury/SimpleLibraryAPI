using FluentValidation;
using SimpleLibrary.API.Requests.Authentication;
using SimpleLibrary.Domain.Enumerations;

namespace SimpleLibrary.API.Validators;

public class AddUserRequestValidator : AbstractValidator<AddUserRequest>
{
    public AddUserRequestValidator()
    {
        RuleFor(x => x.Firstname)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.Lastname)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Passwords do not match.");

        RuleFor(x => x.Role)
            .IsEnumName(typeof(UserType)).When(x => !string.IsNullOrWhiteSpace(x.Role))
            .WithMessage($"Invalid Role value. Available values: {Enum.GetNames(typeof(UserType))}");
    }
}