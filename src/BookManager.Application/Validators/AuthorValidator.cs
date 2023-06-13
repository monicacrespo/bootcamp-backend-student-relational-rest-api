namespace BookManager.Application.Validators
{
    using BookManager.Application.Models;
    using FluentValidation;
    using System.Globalization;
    using System.Text.RegularExpressions;

    public class AuthorValidator : AbstractValidator<Author>
    {
        public AuthorValidator()
        {
            RuleFor(a => a.FirstName)
                .NotEmpty()
                .WithMessage("{PropertyName} should not be empty")
                .Length(2, 100)
                .WithMessage("{PropertyName} should have between {MinLength} and {MaxLength} characters");

            RuleFor(a => a.LastName)
                .NotEmpty()
                .WithMessage("{PropertyName} should not be empty")
                .Length(2, 100)
                .WithMessage("{PropertyName} should have between {MinLength} and {MaxLength} characters");

            RuleFor(a => a.CountryCode)
               .Must(IsNullOrTwoAlphanumericsCharacters)
               .WithMessage("{PropertyName} is null or should have 2 character alpha ISO 3166 code");
        }

        public bool IsNullOrTwoAlphanumericsCharacters(string? countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode?.Trim()))
            {
                return true;
            }
            if (countryCode.Length == 2 && Regex.IsMatch(countryCode, "[a-zA-Z]"))
            {
                try
                {
                    new RegionInfo(countryCode);
                    return true;
                }
                catch (ArgumentException)
                {
                    return false;
                }                
            }
            return false;
        }
    }
}
