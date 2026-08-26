using System.Linq.Expressions;
using ExamSystem.DTOs.Common;

namespace ExamSystem.Extensions
{
    public static class IQueryableExtension
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, BaseFilterDto filter)
        {
            return query.Skip((filter.page_number - 1) * filter.page_size).Take(filter.page_size);
        }

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }
    }
}
