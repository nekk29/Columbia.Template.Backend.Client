namespace __NAMESPACE__.Entity.Base
{
    public class SearchResult<TEntity>
    {
        public int Total { get; set; }
        public IEnumerable<TEntity> Items { get; set; }

        public SearchResult() => Items = new List<TEntity>();
        public SearchResult(IEnumerable<TEntity> items) => Items = items;
    }
}
