namespace __NAMESPACE__.Dto.Base
{
    public class SearchParamsDto
    {
        public PageParamsDto? Page { get; set; }
        public IEnumerable<SortParamsDto>? Sort { get; set; }
    }

    public class SearchParamsDto<TFilter> : SearchParamsDto
    {
        public TFilter? Filter { get; set; }
    }
}
