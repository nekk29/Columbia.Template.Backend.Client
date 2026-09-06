using __NAMESPACE__.Dto.Base;

namespace __NAMESPACE__.Domain.Queries.Base
{
    public class SearchQueryBase<TFilter, TResponse>(SearchParamsDto<TFilter> searchParams) : QueryBase<SearchResultDto<TResponse>>
    {
        public SearchParamsDto<TFilter> SearchParams { get; set; } = searchParams;
    }
}
