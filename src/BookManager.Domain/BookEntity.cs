namespace BookManager.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BookEntity
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public DateTime? PublishedOn { get; set; }

        public string Description { get; set; } = null!;

        // One-to-many relation with author
        public AuthorEntity Author { get; set; } = null!;

        public int AuthorId { get; set; }
    }
}