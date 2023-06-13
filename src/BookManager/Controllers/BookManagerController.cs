namespace BookManager.Controllers
{
    using BookManager.Application;
    using BookManager.Application.Models;
    using BookManager.Application.Validators;
    using BookManager.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;
    using System.Net;
    using System.Net.Mime;

    [Route("api")]
    public class BookManagerController
            : ControllerBase
    {
        private readonly IBookManagerService _bookManagerService;
        public BookManagerController(IBookManagerService bookManagerService)
        {
            _bookManagerService = bookManagerService;
        }

        [BasicAuth]
        [HttpPost("authors")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BookManagerErrorResponse),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAuthor([FromBody] Author author)
        {
            var validator = new AuthorValidator();

            var result = await validator.ValidateAsync(author).ConfigureAwait(false);

            if (!result.IsValid)
            {
                return BadRequest(new BookManagerErrorResponse
                    (result, HttpStatusCode.BadRequest, "The author's data is not valid"));
            }

            try
            {
                var id = await this._bookManagerService.CreateAuthor(author);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }            
        }

        [HttpPost("books")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BookManagerErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [BasicAuth]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            var validator = new BookValidator();

            var result = await validator.ValidateAsync(book).ConfigureAwait(false);

            if (!result.IsValid)
            {
                return BadRequest(new BookManagerErrorResponse
                    (result, HttpStatusCode.BadRequest, "The book's data is not valid"));
            }
            try
            {
                var id = await _bookManagerService.CreateBook(book);

                return Ok(id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }            
        }

        [BasicAuth]
        [HttpPut("books/{id:int}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
        {
            if (await _bookManagerService.DoesExistBook(id) == -1)
            {
                return NotFound();
            }

            if (book.Title.IsNullOrEmpty() && book.Description.IsNullOrEmpty())
            {
                return BadRequest(); // nothing to update
            }

            try
            {
                var resultStatus = await _bookManagerService.UpdateBook(id, book.Title, book.Description);

                return Ok(book);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [BasicAuth]
        [HttpGet("books")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllBooks(string title, string author)
        {
            try
            {
                var allBooks = await _bookManagerService.GetAllBooksIncludingAuthor(title, author);

                return Ok(allBooks);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }            
        }
    }
}