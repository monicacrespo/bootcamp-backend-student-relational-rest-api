namespace BookManager.Application.UnitTests.Validator
{
    using BookManager.Application.Validators;
    using BookManager.Application.Models;
    public class AuthorValidatorTests
    {
        private readonly Author _author;
        private readonly AuthorValidator _authorValidator;

        public AuthorValidatorTests()
        {
            // Do "global" initialization here
            this._authorValidator = new AuthorValidator();
            this._author = new Author
            {
                FirstName = "firstName",
                LastName = "lastName",
                CountryCode = "ES"
            };
        }       

        [Theory]
        [InlineData("ES", true)]
        [InlineData("XX", false)]        
        [InlineData("ESS", false)]
        [InlineData(null, true)]
        [InlineData("", true)]
        public void Given_CountryCode_The_Validation_Is_As_Expected(string? countryCode, bool expected)
        {
            var result = this._authorValidator.IsNullOrTwoAlphanumericsCharacters(countryCode);

            Assert.Equal(expected, result);
        }
        
    }
}
