using __NAMESPACE__.Domain.Commands.Base;
using __NAMESPACE__.Dto.Setting;

namespace __NAMESPACE__.Domain.Commands.Setting
{
    public class CreateSettingCommand(CreateSettingDto createDto) : CommandBase<GetSettingDto>
    {
        public CreateSettingDto CreateDto { get; set; } = createDto;
    }
}
