namespace BookManager.Persistence.SqlServer
{
    using BookManager.Application;
    using BookManager.Domain;
    using Microsoft.EntityFrameworkCore;
   
    public class BookManagerDbContext: DbContext, IBookManagerDbContext
    {
        public BookManagerDbContext(DbContextOptions<BookManagerDbContext> options)
        : base(options)
        {
        }

        public DbSet<BookEntity> Books { get; set; } = null!;
        public DbSet<AuthorEntity> Authors { get; set; } = null!;
       
        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AuthorEntity>()
              .HasKey(x => x.Id);

            modelBuilder
                .Entity<AuthorEntity>()
                .Property(m => m.FirstName)
                .IsRequired()                
                .HasMaxLength(100);

            modelBuilder
              .Entity<AuthorEntity>()
              .Property(m => m.LastName)
              .IsRequired()
              .HasMaxLength(100);

            modelBuilder.Entity<AuthorEntity>()
               .Property(u => u.FullName)
               .HasComputedColumnSql("[FirstName] + ' ' + [LastName]");

            modelBuilder.Entity<AuthorEntity>()
               .HasIndex(m => new { m.FirstName, m.LastName })
               .IsUnique();            

            modelBuilder
              .Entity<AuthorEntity>()
              .Property(m => m.CountryCode)             
              .HasMaxLength(2);

            modelBuilder.Entity<BookEntity>()
               .HasKey(x => x.Id);

            modelBuilder
                .Entity<BookEntity>()
                .HasOne(m => m.Author);

            modelBuilder.Entity<BookEntity>()
               .Property(m => m.Title)                
               .IsRequired()
               .HasMaxLength(150);

            modelBuilder.Entity<BookEntity>()               
               .HasIndex(m => m.Title)
               .IsUnique();

            modelBuilder.Entity<BookEntity>()
                .Property(m => m.Description)
                .IsRequired()
                .HasMaxLength(450);

            modelBuilder.Entity<BookEntity>()
                .HasOne(m => m.Author)
                .WithMany("Books")
                .HasForeignKey("AuthorId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}