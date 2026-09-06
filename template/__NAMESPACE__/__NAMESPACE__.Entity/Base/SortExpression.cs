using System.Linq.Expressions;

namespace __NAMESPACE__.Entity.Base
{
    public class SortExpression<TEntity>
    {
        public SortDirection Direction { get; set; }
        public Expression<Func<TEntity, object>>? Property { get; set; }
    }
}
