using __NAMESPACE__.Domain.Queries.Base;
using __NAMESPACE__.Dto.Base;
using __NAMESPACE__.Dto.Setting;

namespace __NAMESPACE__.Domain.Queries.Setting
{
    public class SearchSettingQuery(SearchParamsDto<SearchSettingFilterDto> searchParams) : SearchQueryBase<SearchSettingFilterDto, SearchSettingDto>(searchParams)
    {

    }
}
