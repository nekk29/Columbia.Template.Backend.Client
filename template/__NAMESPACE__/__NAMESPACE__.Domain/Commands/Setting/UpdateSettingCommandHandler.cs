using AutoMapper;
using __NAMESPACE__.Common;
using __NAMESPACE__.Domain.Commands.Base;
using __NAMESPACE__.Dto.Base;
using __NAMESPACE__.Dto.Setting;
using __NAMESPACE__.Repository.Abstractions.Base;
using __NAMESPACE__.Repository.Abstractions.Transactions;
using Microsoft.Extensions.Configuration;

namespace __NAMESPACE__.Domain.Commands.Setting
{
    public class UpdateSettingCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IConfiguration configuration,
        UpdateSettingCommandValidator validator,
        IRepository<Entity.Setting> settingRepository
    ) : CommandHandlerBase<UpdateSettingCommand, GetSettingDto>(unitOfWork, mapper, validator)
    {
        protected override bool UseTransaction => false;

        public override async Task<ResponseDto<GetSettingDto>> HandleCommand(UpdateSettingCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetSettingDto>();
            var securityKey = configuration.GetValue<string>("SecurityOptions:SecurityKey");

            var setting = await settingRepository.GetByAsync(x => x.Group == request.UpdateDto.Group && x.Code == request.UpdateDto.Code);
            if (setting != null)
            {
                setting.Value = request.UpdateDto.Value;
                setting.Description = request.UpdateDto.Description;
                setting.IsActive = request.UpdateDto.IsActive;

                if (Constants.Settings.EncryptedSettings.Any(x => x.Group == setting.Group && x.Code == setting.Code))
                    setting.Value = Encrypter.Encrypt(setting.Value, securityKey!);

                await settingRepository.UpdateAsync(setting);

                /* Clean memory cache setting items
                _memoryCache.Remove(Constants.Cache.{CacheKey});
                */
            }

            var settingDto = _mapper?.Map<GetSettingDto>(setting);
            if (settingDto != null) response.UpdateData(settingDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
