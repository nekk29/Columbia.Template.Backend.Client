using __NAMESPACE__.Dto.Base;
using MediatR;

namespace __NAMESPACE__.Domain.Queries.Base
{
    public class QueryBase : IRequest<ResponseDto>
    {

    }

    public class QueryBase<TResponse> : IRequest<ResponseDto<TResponse>>
    {

    }
}
