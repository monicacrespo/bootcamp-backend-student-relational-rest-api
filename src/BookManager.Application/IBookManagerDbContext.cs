namespace BookManager.Application
{
    using Microsoft.EntityFrameworkCore;
    using BookManager.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IBookManagerDbContext
    {
        DbSet<BookEntity> Books { get; }
        DbSet<AuthorEntity> Authors { get; }
      
        Task<int> SaveChangesAsync();
    }
}