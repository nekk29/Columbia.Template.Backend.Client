using AutoMapper;
using __NAMESPACE__.Domain.Commands.Base;
using __NAMESPACE__.Dto.Base;
using __NAMESPACE__.Dto.Setting;
using __NAMESPACE__.Repository.Abstractions.Base;
using __NAMESPACE__.Repository.Abstractions.Transactions;

namespace __NAMESPACE__.Domain.Commands.Setting
{
    public class CreateSettingCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        CreateSettingCommandValidator validator,
        IRepository<Entity.Setting> settingRepository
    ) : CommandHandlerBase<CreateSettingCommand, GetSettingDto>(unitOfWork, mapper, validator)
    {
        protected override bool UseTransaction => false;

        public override async Task<ResponseDto<GetSettingDto>> HandleCommand(CreateSettingCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetSettingDto>();
            var setting = _mapper?.Map<Entity.Setting>(request.CreateDto);

            if (setting != null)
                await settingRepository.AddAsync(setting);

            var settingDto = _mapper?.Map<GetSettingDto>(setting);
            if (settingDto != null) response.UpdateData(settingDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
