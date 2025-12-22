// Validators/AdvertCreateDTOValidator.cs
using AnimalMarketplace.Database.DTO;
using FluentValidation;

namespace AnimalMarketplace.Validators;

public class AdvertCreateDtoValidator : AbstractValidator<AdvertCreateDto>
{
    public AdvertCreateDtoValidator()
    {
        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price cannot be less than 0");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description cannot be empty")
            .MaximumLength(500)
            .WithMessage("Description can be at most 500 characters");
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title cannot be empty")
            .MaximumLength(50)
            .WithMessage("Title can be at most 50 characters");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City cannot be empty")
            .MaximumLength(50)
            .WithMessage("City can be at most 50 characters");

        RuleFor(x => x.District)
            .NotEmpty()
            .WithMessage("District cannot be empty")
            .MaximumLength(50)
            .WithMessage("District can be at most 50 characters");

        RuleFor(x => x.Age)
            .InclusiveBetween(0, 100)
            .WithMessage("Age must be between 0 and 100");

        RuleFor(x => x.Gender)
            .IsInEnum()
            .WithMessage("Please select a valid gender");

        RuleFor(x => x.Kind)
            .IsInEnum()
            .WithMessage("Please select a valid animal type");
    }
}
