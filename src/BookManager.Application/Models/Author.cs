namespace BookManager.Application.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Author
    {
        public string FirstName { get; set; } = String.Empty;

        public string LastName { get; set; } = String.Empty;

        public string FullName { get; private set; } = String.Empty;

        public DateTime? DateOfBirth { get; set; }

        public string? CountryCode { get; set; } = String.Empty;
    }
}