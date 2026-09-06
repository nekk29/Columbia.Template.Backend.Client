using __NAMESPACE__.Dto.Base;
using __NAMESPACE__.Dto.Setting;

namespace __NAMESPACE__.RestClient.Abstractions
{
    public interface ISettingRestService
    {
        Task<ResponseDto<GetSettingDto>> Create(CreateSettingDto createDto);
        Task<ResponseDto<GetSettingDto>> Update(UpdateSettingDto updateDto);
        Task<ResponseDto> Delete();
        Task<ResponseDto<GetSettingDto>> Get();
        Task<ResponseDto<IEnumerable<ListSettingDto>>> List();
        Task<ResponseDto<SearchResultDto<SearchSettingDto>>> Search(SearchParamsDto<SearchSettingFilterDto> filter);
    }
}
