using System.ComponentModel.DataAnnotations;

namespace BookManager.Application.Models
{
    public class Book
    {
        public string Title { get; set; } = String.Empty;

        public DateTime? PublishedOn { get; set; }

        public string Description { get; set; } = String.Empty;

        public int AuthorId { get; set; }
    }
}