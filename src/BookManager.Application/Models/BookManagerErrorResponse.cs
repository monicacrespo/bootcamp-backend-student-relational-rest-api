using FluentValidation.Results;
using System.Net;

namespace BookManager.Application.Models
{
    public class BookManagerErrorResponse
    {
        public Dictionary<string, IEnumerable<string>> Errors { get; set; }

        public int HttpStatusCode { get; set; }

        public string Message { get; set; }

        public BookManagerErrorResponse(ValidationResult validationResult, HttpStatusCode httpStatusCode, string message)
        {
            Errors = new Dictionary<string, IEnumerable<string>>();

            foreach (var propertyName in validationResult.Errors.Select(e => e.PropertyName).Distinct())
            {
                var errorMessages = validationResult.Errors.Where(e => e.PropertyName == propertyName).Select(m => m.ErrorMessage).ToList();
                Errors.Add(propertyName, errorMessages);
            }

            HttpStatusCode = (int)httpStatusCode;

            Message = message;
        }
    }
}
