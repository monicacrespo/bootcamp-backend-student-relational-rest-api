namespace BookManager.Application.Validators
{
    using BookManager.Application.Models;
    using FluentValidation;
    using System.Globalization;
    using System.Text.RegularExpressions;
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(a => a.Title)
               .NotEmpty()
               .WithMessage("{PropertyName} should not be empty")
               .Length(2, 150)
               .WithMessage("{PropertyName} should have between {MinLength} and {MaxLength} characters");

            RuleFor(a => a.Description)
               .NotEmpty()
               .WithMessage("{PropertyName} should not be empty")
               .Length(2, 450)
               .WithMessage("{PropertyName} should have between {MinLength} and {MaxLength} characters");          
        }
    }
}
