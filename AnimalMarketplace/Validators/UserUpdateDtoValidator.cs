using System.Text.RegularExpressions;
using AnimalMarketplace.Database.Dto;
using FluentValidation;

namespace AnimalMarketplace.Validators;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
    // --- 1. Name Field Validation ---
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("The Name field cannot be empty.")
            .MinimumLength(2).WithMessage("The name must be at least 2 characters long.")
            .MaximumLength(50).WithMessage("The name cannot exceed 50 characters.");

        // --- 2. Surname Field Validation ---
        RuleFor(user => user.Surname)
            .NotEmpty().WithMessage("The Surname field cannot be empty.")
            .MinimumLength(2).WithMessage("The surname must be at least 2 characters long.")
            .MaximumLength(50).WithMessage("The surname cannot exceed 50 characters.");

        // --- 3. Email Field Validation ---
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("The Email field cannot be empty.")
            .EmailAddress().WithMessage("Please enter a valid email address.")
            .MaximumLength(100).WithMessage("The email cannot exceed 100 characters.");

        // --- 4. Phone Field Validation (Optional RegEx) ---
        RuleFor(user => user.Phone)
            .NotEmpty().WithMessage("The Phone Number field cannot be empty.")
            .Must(BeValidPhoneNumber).WithMessage("Please enter a valid phone number format (e.g., +905551234567).")
            .MaximumLength(20).WithMessage("The phone number cannot exceed 20 characters.");

        // --- 5. Gender Field Validation ---
        RuleFor(user => user.Gender)
            .IsInEnum().WithMessage("Please select a valid gender value.");
    }

    /// <summary>
    /// Custom method to validate the phone number format.
    /// </summary>
    private bool BeValidPhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        // Simple RegEx: Allows digits, spaces, '-', '+', '(', ')'
        var regex = new Regex(@"^[\d\s\-\+\(\)]+$");
        return regex.IsMatch(phone);
    }
}