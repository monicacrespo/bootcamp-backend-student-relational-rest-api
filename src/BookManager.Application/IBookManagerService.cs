namespace BookManager.Application
{
    using BookManager.Application.Models;
    public interface IBookManagerService    
    {
        Task<int> CreateAuthor(Author author);

        Task<int> CreateBook(Book book);

        Task<int> UpdateBook(int id, string? title, string? description);

        Task<string> GetAllBooksIncludingAuthor(string title, string author);

        Task<int> DoesExistBook(int bookId);
    }
}