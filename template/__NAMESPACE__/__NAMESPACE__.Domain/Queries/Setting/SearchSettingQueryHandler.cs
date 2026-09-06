using System.Linq.Expressions;
using AutoMapper;
using __NAMESPACE__.Common;
using __NAMESPACE__.Domain.Queries.Base;
using __NAMESPACE__.Domain.Services.Setting;
using __NAMESPACE__.Dto.Base;
using __NAMESPACE__.Dto.Setting;
using __NAMESPACE__.Entity.Base;
using __NAMESPACE__.Repository.Abstractions.Base;
using __NAMESPACE__.Repository.Extensions;
using Microsoft.Extensions.Configuration;

namespace __NAMESPACE__.Domain.Queries.Setting
{
    public class SearchSettingQueryHandler(
        IMapper mapper,
        IConfiguration configuration,
        IRepository<Entity.Setting> settingRepository
    ) : SearchQueryHandlerBase<SearchSettingQuery, SearchSettingFilterDto, SearchSettingDto>(mapper)
    {
        protected override async Task<ResponseDto<SearchResultDto<SearchSettingDto>>> HandleQuery(SearchSettingQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<SearchResultDto<SearchSettingDto>>();
            var securityKey = configuration.GetValue<string>("SecurityOptions:SecurityKey");

            Expression<Func<Entity.Setting, bool>> filter = x => true;

            var filters = request.SearchParams?.Filter;

            if (!string.IsNullOrEmpty(filters?.Query))
                filter = filter.And(x => x.Group.Contains(filters.Query) || x.Code.Contains(filters.Query) || x.Description.Contains(filters.Query));

            var sorts = new List<SortExpression<Entity.Setting>>();

            if (request.SearchParams?.Sort?.Any() == true)
            {
                request.SearchParams.Sort.ToList().ForEach(x =>
                {
                    var sort = IQueryableExtensions.GetSortExpression<Entity.Setting>(x.Direction, x.Property);
                    if (sort != null) sorts.Add(sort);
                });
            }
            else
            {
                sorts.Add(new SortExpression<Entity.Setting> { Direction = SortDirection.Asc, Property = x => x.Group });
                sorts.Add(new SortExpression<Entity.Setting> { Direction = SortDirection.Asc, Property = x => x.Code });
            }

            var settings = await settingRepository.SearchByAsNoTrackingAsync(
                request.SearchParams?.Page?.Page ?? 1,
                request.SearchParams?.Page?.PageSize ?? 10,
                sorts,
                filter
            );

            var settingDtos = _mapper?.Map<IEnumerable<SearchSettingDto>>(settings.Items);

            settingDtos ??= new List<SearchSettingDto>();

            foreach (var settingDto in settingDtos ?? [])
            {
                if (Constants.Settings.EncryptedSettings.Any(x => x.Group == settingDto.Group && x.Code == settingDto.Code))
                {
                    settingDto.Encrypted = true;
                    settingDto.Value = SettingService.HideValue(settingDto.Value, securityKey!);
                }
            }

            var searchResult = new SearchResultDto<SearchSettingDto>(
                settingDtos!,
                settings.Total,
                request.SearchParams
            );

            response.UpdateData(searchResult);

            return await Task.FromResult(response);
        }
    }
}
