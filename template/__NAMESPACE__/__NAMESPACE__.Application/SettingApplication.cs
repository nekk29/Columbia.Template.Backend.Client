using __NAMESPACE__.Application.Abstractions;
using __NAMESPACE__.Application.Base;
using __NAMESPACE__.Domain.Commands.Setting;
using __NAMESPACE__.Domain.Queries.Setting;
using __NAMESPACE__.Dto.Base;
using __NAMESPACE__.Dto.Setting;
using MediatR;

namespace __NAMESPACE__.Application
{
    public class SettingApplication(IMediator mediator) : ApplicationBase(mediator), ISettingApplication
    {
        public async Task<ResponseDto<GetSettingDto>> Create(CreateSettingDto createDto)
            => await _mediator.Send(new CreateSettingCommand(createDto));

        public async Task<ResponseDto<GetSettingDto>> Update(UpdateSettingDto updateDto)
            => await _mediator.Send(new UpdateSettingCommand(updateDto));

        public async Task<ResponseDto> Delete(string group, string code)
            => await _mediator.Send(new DeleteSettingCommand(group, code));

        public async Task<ResponseDto<GetSettingDto>> Get(string group, string code)
            => await _mediator.Send(new GetSettingQuery(group, code));

        public async Task<ResponseDto<IEnumerable<ListSettingDto>>> List()
            => await _mediator.Send(new ListSettingQuery());

        public async Task<ResponseDto<SearchResultDto<SearchSettingDto>>> Search(SearchParamsDto<SearchSettingFilterDto> searchParams)
            => await _mediator.Send(new SearchSettingQuery(searchParams));
    }
}
