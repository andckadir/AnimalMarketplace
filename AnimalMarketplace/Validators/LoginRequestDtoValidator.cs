using AnimalMarketplace.Database.DTO;
using FluentValidation;

namespace AnimalMarketplace.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(loginRequest => loginRequest.Email)
            .NotEmpty().WithMessage("The Email field cannot be empty.")
            .EmailAddress().WithMessage("Please enter a valid email address.")
            .MaximumLength(100).WithMessage("The email cannot exceed 100 characters.");
    
        RuleFor(loginRequest => loginRequest.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(50)
            .WithMessage("Password must not exceed 50 characters.")
            .Matches(@"[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]")
            .WithMessage("Password must contain at least one digit.");    
    }
    
}