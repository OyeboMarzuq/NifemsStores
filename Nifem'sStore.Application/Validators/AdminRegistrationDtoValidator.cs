using FluentValidation;
using NifemsStores.Application.DTOs;

namespace NifemsStores.Application.Validators
{
    public class AdminRegistrationDtoValidator : AbstractValidator<AdminRegistrationDto>
    {
        public AdminRegistrationDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstName is required.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required.");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Surname is required.");

            RuleFor(x => x.Avatar)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Avatar is required.")
                .Must(file => file.Length <= 3 * 1024 * 1024)
                    .WithMessage("Avatar must not exceed 3MB.")
                .Must(file =>
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    return allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower());
                })
                    .WithMessage("Allowed image formats: JPG, JPEG, PNG, GIF, WEBP.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.");

            RuleFor(x => x.Nationality)
                .NotEmpty().WithMessage("Nationality is required.");
        }
    }
}
