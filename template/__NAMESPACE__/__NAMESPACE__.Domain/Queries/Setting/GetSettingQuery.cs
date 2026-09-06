using __NAMESPACE__.Domain.Queries.Base;
using __NAMESPACE__.Dto.Setting;

namespace __NAMESPACE__.Domain.Queries.Setting
{
    public class GetSettingQuery(string group, string code) : QueryBase<GetSettingDto>
    {
        public string Group { get; set; } = group;
        public string Code { get; set; } = code;
    }
}
