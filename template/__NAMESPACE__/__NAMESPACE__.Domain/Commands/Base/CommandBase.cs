using __NAMESPACE__.Dto.Base;
using MediatR;

namespace __NAMESPACE__.Domain.Commands.Base
{
    public class CommandBase : IRequest<ResponseDto>
    {
        public bool Validate { get; private set; } = true;
        public void EnableValidation() => Validate = true;
        public void DisableValidation() => Validate = false;
    }

    public class CommandBase<TResponse> : IRequest<ResponseDto<TResponse>>
    {
        public bool Validate { get; private set; } = true;
        public void EnableValidation() => Validate = true;
        public void DisableValidation() => Validate = false;
    }
}
