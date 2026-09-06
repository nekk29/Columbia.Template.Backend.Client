using __NAMESPACE__.Dto.Base;
using __NAMESPACE__.Entity.Base;
using __NAMESPACE__.Repository.Extensions;

namespace __NAMESPACE__.Domain.Extensions
{
    public static class SearchQueryExtensions
    {
        public static IEnumerable<SortExpression<TEntity>> GetSortExpressions<TEntity>(this IEnumerable<SortParamsDto> sorts)
        {
            if (sorts == null) return [];

            var sortExpression = new List<SortExpression<TEntity>>();

            foreach (var sort in sorts ?? [])
            {
                var sortExpr = IQueryableExtensions.GetSortExpression<TEntity>(sort.Direction, sort.Property);
                if (sortExpr != null) sortExpression.Add(sortExpr);
            }

            return sortExpression;
        }
    }
}
