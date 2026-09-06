using AutoMapper;
using __NAMESPACE__.Common;
using __NAMESPACE__.Domain.Queries.Base;
using __NAMESPACE__.Domain.Services.Setting;
using __NAMESPACE__.Dto.Base;
using __NAMESPACE__.Dto.Setting;
using __NAMESPACE__.Repository.Abstractions.Base;
using Microsoft.Extensions.Configuration;

namespace __NAMESPACE__.Domain.Queries.Setting
{
    public class GetSettingQueryHandler(
        IMapper mapper,
        IConfiguration configuration,
        IRepository<Entity.Setting> settingRepository
    ) : QueryHandlerBase<GetSettingQuery, GetSettingDto>(mapper)
    {
        protected override async Task<ResponseDto<GetSettingDto>> HandleQuery(GetSettingQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetSettingDto>();
            var setting = await settingRepository.GetByAsync(x => x.Group == request.Group && x.Code == request.Code);
            var settingDto = _mapper?.Map<GetSettingDto>(setting);

            if (settingDto != null)
            {
                if (Constants.Settings.EncryptedSettings.Any(x => x.Group == settingDto.Group && x.Code == settingDto.Code))
                {
                    var securityKey = configuration.GetValue<string>("SecurityOptions:SecurityKey");

                    settingDto.Encrypted = true;
                    settingDto.Value = SettingService.HideValue(settingDto.Value, securityKey!);
                }

                response.UpdateData(settingDto);
            }

            return await Task.FromResult(response);
        }
    }
}
