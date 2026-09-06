using __NAMESPACE__.Entity.Base;

namespace __NAMESPACE__.Entity
{
    public partial class Setting : SystemEntity
    {
        public string Group { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
