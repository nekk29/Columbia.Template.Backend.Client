using AutoMapper;
using __NAMESPACE__.Dto.Setting;

namespace __NAMESPACE__.Domain.Mapping
{
    public class SettingProfile : Profile
    {
        public SettingProfile()
        {
            CreateMap<Entity.Setting, SettingDto>()
                .ReverseMap();

            CreateMap<Entity.Setting, UpdateSettingDto>()
                .ReverseMap();

            CreateMap<Entity.Setting, GetSettingDto>()
                .ReverseMap();

            CreateMap<Entity.Setting, ListSettingDto>()
                .ReverseMap();

            CreateMap<Entity.Setting, SearchSettingDto>()
                .ReverseMap();
        }
    }
}
