namespace __NAMESPACE__.Dto.Base
{
    public class SearchResultDto<TResult>
    {
        public SearchResultDto(IEnumerable<TResult> items)
        {
            Items = items;
            Total = Items.Count();
            Page = 1;
            PageSize = Items.Count();
        }

        public SearchResultDto(IEnumerable<TResult> items, int total) : this(items)
        {
            Total = total > Items.Count() ? total : Items.Count();
            Page = 1;
            PageSize = Items.Count();
        }

        public SearchResultDto(IEnumerable<TResult> items, int total, int page, int pageSize) : this(items, total)
        {
            Page = page;
            PageSize = pageSize;
        }

        public SearchResultDto(IEnumerable<TResult> items, int total, SearchParamsDto? searchParams) : this(items, total)
        {
            Page = searchParams?.Page?.Page ?? 1;
            PageSize = searchParams?.Page?.PageSize ?? Items.Count();
        }

        public IEnumerable<TResult> Items { get; set; } = new List<TResult>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
