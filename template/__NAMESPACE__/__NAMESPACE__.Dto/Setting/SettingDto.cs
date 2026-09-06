namespace __NAMESPACE__.Dto.Setting
{
    public class SettingDto
    {
        public string Group { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Value { get; set; } = null!;
        public bool Encrypted { get; set; }
    }
}
