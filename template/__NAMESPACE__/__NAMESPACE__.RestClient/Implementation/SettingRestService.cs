using __NAMESPACE__.Dto.Base;
using __NAMESPACE__.Dto.Setting;
using __NAMESPACE__.RestClient.Abstractions;
using __NAMESPACE__.RestClient.Base;

namespace __NAMESPACE__.RestClient.Implementation
{
    public class SettingRestService(IServiceProvider serviceProvider) : BaseService(serviceProvider), ISettingRestService
    {
        protected override string ApiController => "api/setting";

        public async Task<ResponseDto<GetSettingDto>> Create(CreateSettingDto createDto)
            => await Post<CreateSettingDto, ResponseDto<GetSettingDto>>(string.Empty, createDto)!;

        public async Task<ResponseDto<GetSettingDto>> Update(UpdateSettingDto updateDto)
            => await Put<UpdateSettingDto, ResponseDto<GetSettingDto>>(string.Empty, updateDto)!;

        public async Task<ResponseDto> Delete()
            => await Delete<ResponseDto>()!;

        public async Task<ResponseDto<GetSettingDto>> Get()
            => await Get<ResponseDto<GetSettingDto>>()!;

        public async Task<ResponseDto<IEnumerable<ListSettingDto>>> List()
            => await Get<ResponseDto<IEnumerable<ListSettingDto>>>("/list")!;

        public async Task<ResponseDto<SearchResultDto<SearchSettingDto>>> Search(SearchParamsDto<SearchSettingFilterDto> filter)
            => await Post<SearchParamsDto<SearchSettingFilterDto>, ResponseDto<SearchResultDto<SearchSettingDto>>>("/search", filter)!;
    }
}
