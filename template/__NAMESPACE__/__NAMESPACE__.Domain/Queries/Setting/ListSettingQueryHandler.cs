using AutoMapper;
using __NAMESPACE__.Domain.Queries.Base;
using __NAMESPACE__.Dto.Base;
using __NAMESPACE__.Dto.Setting;
using __NAMESPACE__.Repository.Abstractions.Base;
using Microsoft.EntityFrameworkCore;

namespace __NAMESPACE__.Domain.Queries.Setting
{
    public class ListSettingQueryHandler(
        IMapper mapper,
        IRepository<Entity.Setting> settingRepository
    ) : QueryHandlerBase<ListSettingQuery, IEnumerable<ListSettingDto>>(mapper)
    {
        protected override async Task<ResponseDto<IEnumerable<ListSettingDto>>> HandleQuery(ListSettingQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListSettingDto>>();
            var items = await settingRepository.FindAll().ToListAsync(cancellationToken);
            var itemDtos = _mapper?.Map<IEnumerable<ListSettingDto>>(items);

            response.UpdateData(itemDtos ?? []);

            return await Task.FromResult(response);
        }
    }
}
