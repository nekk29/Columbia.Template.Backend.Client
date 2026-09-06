using __NAMESPACE__.Domain.Commands.Base;
using __NAMESPACE__.Dto.Setting;

namespace __NAMESPACE__.Domain.Commands.Setting
{
    public class UpdateSettingCommand(UpdateSettingDto updateDto) : CommandBase<GetSettingDto>
    {
        public UpdateSettingDto UpdateDto { get; set; } = updateDto;
    }
}
