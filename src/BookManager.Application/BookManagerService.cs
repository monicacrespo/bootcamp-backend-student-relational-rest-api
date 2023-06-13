namespace BookManager.Application
{
    using BookManager.Application.Models;
    using BookManager.Domain;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class BookManagerService : IBookManagerService
    {
        private readonly IBookManagerDbContext _bookManagerDbContext;

        public BookManagerService(IBookManagerDbContext bookManagerDbContext)
        {
            _bookManagerDbContext = bookManagerDbContext;
        }

        public async Task<int> CreateAuthor(Author author)
        {
            AuthorEntity authorEntity = new AuthorEntity
            {
                FirstName = author.FirstName,
                LastName = author.LastName,
                DateOfBirth = author.DateOfBirth,
                CountryCode = author.CountryCode
            };                    

            _bookManagerDbContext.Authors.Add(authorEntity);
            await _bookManagerDbContext.SaveChangesAsync();

            return authorEntity.Id;
        }
        public async Task<int> CreateBook(Book book)
        {
            var bookEntity = new BookEntity
            {
                Title = book.Title,
                PublishedOn = book.PublishedOn,
                Description = book.Description,
                AuthorId = book.AuthorId
            };

            _bookManagerDbContext.Books.Add(bookEntity);
            await _bookManagerDbContext.SaveChangesAsync();

            return bookEntity.Id;
        }
       
        public async Task<int> UpdateBook(int id, string? title, string? description)
        {
            BookEntity? bookEntity = _bookManagerDbContext.Books.ToList().Find(x => x.Id == id);

            if (bookEntity == null)
            {
                return -1; 
            }

            if (!string.IsNullOrWhiteSpace(title))
            { 
                bookEntity.Title = title; 
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                bookEntity.Description = description;
            }

            _bookManagerDbContext.Books.Update(bookEntity);
            await _bookManagerDbContext.SaveChangesAsync();

            return id;
        }

        public async Task<string> GetAllBooksIncludingAuthor(string title, string author)
        {
            Expression<Func<BookEntity, bool>> predicate = (x) => true;

            // if the title is null or empty will return all books
            if (!string.IsNullOrWhiteSpace(title))
            {
                predicate = (x => x.Title.ToLower() == title.ToLower()); // query books with specific title
            }

            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(author))
            // query books with specific title AND whose FullName (FirstName + ' ' + LastName) contains the value passed in the filter
            {
                predicate = (x => x.Title.ToLower() == title.ToLower() && x.Author.FullName.ToLower().Contains(author.ToLower()));
            }

            var booksIncludingAuthor = await _bookManagerDbContext.Books   
                .OrderBy(b => b.Title)
                .Where(predicate)
                .Select(b => new
                     {
                         Id = b.Id,
                         Title = b.Title,
                         Description = b.Description,
                         Author = b.Author.FullName
                     })
                .ToListAsync();           

            // serialize a list of anonimous objects
           var json = JsonSerializer.Serialize<object>(booksIncludingAuthor, new JsonSerializerOptions { WriteIndented = true, });
            
           return json;
        }

        public async Task<int> DoesExistBook(int bookId)
        {
            BookEntity? bookEntity = await _bookManagerDbContext
                .Books
                .FindAsync(bookId);

            return bookEntity == null ? -1 : 0;
        }
    }
}