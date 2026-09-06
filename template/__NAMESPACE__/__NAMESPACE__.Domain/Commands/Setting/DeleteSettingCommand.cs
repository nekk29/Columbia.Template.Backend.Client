using __NAMESPACE__.Domain.Commands.Base;

namespace __NAMESPACE__.Domain.Commands.Setting
{
    public class DeleteSettingCommand(string group, string code) : CommandBase
    {
        public string Group { get; set; } = group;
        public string Code { get; set; } = code;
    }
}
