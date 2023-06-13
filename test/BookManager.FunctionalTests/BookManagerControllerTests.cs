namespace BookManager.FunctionalTests {

    using System.Net;
    using System.Net.Http.Headers;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using BookManager.Application;
    using BookManager.Application.Models;
    using BookManager.Application.Validators;
    using BookManager.FunctionalTests.TestSupport;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
    using Moq;
    using Microsoft.Extensions.Primitives;
    using Microsoft.Net.Http.Headers;

    public class BookManagerControllerTests : IntegrationTest
    {
        [Fact]
        public async Task Given_Author_When_CreateAuthor_Then_It_Returns_200_Ok()
        {
            // Given
            var newAuthor = new Author
            {
                FirstName = "FirstNameTest",
                LastName = "LastNameTest",
                DateOfBirth = DateTime.Now,
                CountryCode = "ES"
            };           
           
            // When
            var result = await HttpClient.PostAsJsonAsync($"api/authors", newAuthor);

            // Then
            var payload = await result.Content.ReadAsStringAsync();
            payload.Should().Contain("1");        
            result.StatusCode.Should().Be(HttpStatusCode.OK);            
        }
        [Fact]
        public async Task Given_Author_When_CreateAuthor_With_Invalid_Credentials_Then_It_Returns_401()
        {
            // Given
            var newAuthor = new Author
            {
                FirstName = "FirstNameTest",
                LastName = "LastNameTest",
                DateOfBirth = DateTime.Now,
                CountryCode = "ES"
            };
           
            HttpClient.DefaultRequestHeaders.Remove("Authorization");

            // When
            var result = await HttpClient.PostAsJsonAsync($"api/authors", newAuthor);

            // Then          
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Given_Author_And_Two_Books_When_Getting_All_Books_Then_All_expected_books_Are_Retrieved()
        {
            // Given

            // Step 1: Create author
            var newAuthor = new Author
            {
                FirstName = "Jhon",
                LastName = "Sandford",
                DateOfBirth = DateTime.Now,
                CountryCode = "US"
            };
            var createAuthorResponse = await HttpClient.PostAsJsonAsync($"api/authors", newAuthor);
            if (!createAuthorResponse.IsSuccessStatusCode)
            {
                throw new Exception("Could not create author. Test cannot continue");
            }
            var createAuthorResponseText = await createAuthorResponse.Content.ReadAsStringAsync(); // e.g: "1"
            // Convert to int

            if (!int.TryParse(createAuthorResponseText, out int authorId))
            {
                throw new Exception("Could not parse new author identifier. Test cannot continue");
            }

            // Step 2: Create 2 books
            var bookOnePayload = new Book
            {
                Title = "Todo va a mejorar",
                PublishedOn = DateTime.Now.AddYears(-1),
                Description = "Galería inolvidable de personajes, que van contando su experiencia de adaptación a un país que ha sufrido fuertes sacudidas y en el que no quieren resignarse.",
                AuthorId = authorId
            };
            var bookTwoPayload = new Book
            {
                Title = "Deadline",
                PublishedOn = DateTime.Now.AddYears(-2),
                Description = "In Southeast Minnesota, down on the Mississippi, a school board meeting is coming to an end. The board chairman announces that the rest of the meeting will be closed, due to personnel issues. \"Issues\" is correct. The proposal up for a vote before them is whether to authorize the killing of a local reporter. The vote is four to one in favor.",
                AuthorId = authorId
            };

            var bookOneResponse = await HttpClient.PostAsJsonAsync($"api/books", bookOnePayload);
            var bookTwoResponse = await HttpClient.PostAsJsonAsync($"api/books", bookTwoPayload);
            if (!bookOneResponse.IsSuccessStatusCode || !bookTwoResponse.IsSuccessStatusCode)
            {
                throw new Exception("Could not create book. Test cannot continue");
            }

            // When
            var result = await HttpClient.GetAsync($"api/books");

            // Then
            var json = await result.Content.ReadAsStringAsync();
            var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            var books = (JsonSerializer.Deserialize<IEnumerable<Book>>(json, serializerOptions)
                           ?? Array.Empty<Book>()).ToList();

            books.Should().HaveCount(2);
        }
    }
}