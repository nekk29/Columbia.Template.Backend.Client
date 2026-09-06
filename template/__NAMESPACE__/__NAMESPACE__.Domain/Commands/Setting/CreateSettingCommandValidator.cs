using __NAMESPACE__.Domain.Commands.Base;

namespace __NAMESPACE__.Domain.Commands.Setting
{
    public class CreateSettingCommandValidator : CommandValidatorBase<CreateSettingCommand>
    {
        public CreateSettingCommandValidator()
        {
            RequiredInformation(x => x.CreateDto)
                .DependentRules(() =>
                {
                    //RequiredString(x => x.CreateDto.Group, Resources.Setting.Group, {Min}, {Max});
                    //RequiredString(x => x.CreateDto.Code, Resources.Setting.Code, {Min}, {Max});
                    //RequiredString(x => x.CreateDto.Description, Resources.Setting.Description, {Min}, {Max});
                    //RequiredString(x => x.CreateDto.Value, Resources.Setting.Value, {Min}, {Max});
                });
        }
    }
}
