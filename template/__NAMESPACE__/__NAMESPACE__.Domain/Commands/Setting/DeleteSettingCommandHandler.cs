using AutoMapper;
using __NAMESPACE__.Domain.Commands.Base;
using __NAMESPACE__.Dto.Base;
using __NAMESPACE__.Repository.Abstractions.Base;
using __NAMESPACE__.Repository.Abstractions.Transactions;

namespace __NAMESPACE__.Domain.Commands.Setting
{
    public class DeleteSettingCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        DeleteSettingCommandValidator validator,
        IRepository<Entity.Setting> settingRepository
    ) : CommandHandlerBase<DeleteSettingCommand>(unitOfWork, mapper, validator)
    {
        protected override bool UseTransaction => false;

        public override async Task<ResponseDto> HandleCommand(DeleteSettingCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            await settingRepository.DeleteAsync();

            response.AddOkResult(Resources.Common.DeleteSuccessMessage);

            return response;
        }
    }
}
