using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExamSystem.DTOs;

namespace ExamSystem.Utilities
{
    public class PaginationHelper
    {
        public static Paginated<T> Paginated<T>(List<T> items, int pageNumber, int pageSize, int totalItems, HttpRequest request)
        {
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var baseUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            return new Paginated<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasNextPage = pageNumber < totalPages,
                HasPreviousPage = pageNumber > 1
            };
        }
    }
}
