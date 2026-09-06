using AutoMapper;
using __NAMESPACE__.Dto.Base;
using MediatR;

namespace __NAMESPACE__.Domain.Queries.Base
{
    public abstract class QueryHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, ResponseDto<TResponse>> where TRequest : QueryBase<TResponse>
    {
        protected QueryValidatorBase<TRequest>? _validator;

        protected readonly IMapper? _mapper;
        protected readonly IMediator? _mediator;

        public QueryHandlerBase()
        {

        }

        public QueryHandlerBase(IMapper mapper)
            => _mapper = mapper;

        public QueryHandlerBase(IMapper mapper, IMediator mediator) : this(mapper)
            => _mediator = mediator;

        public QueryHandlerBase(IMapper mapper, QueryValidatorBase<TRequest> validator) : this(mapper)
            => _validator = validator;

        public QueryHandlerBase(IMapper mapper, IMediator mediator, QueryValidatorBase<TRequest> validator) : this(mapper)
        {
            _mediator = mediator;
            _validator = validator;
        }

        public async Task<ResponseDto<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
            => await ValidateAndHandle(request, cancellationToken);

        protected virtual async Task<ResponseDto<TResponse>> ValidateAndHandle(TRequest request, CancellationToken cancellationToken)
        {
            if (_validator != null)
            {
                var result = await _validator.ValidateAsync(request, cancellationToken);

                if (!result.IsValid)
                {
                    var response = new ResponseDto<TResponse>();
                    response.AddErrorResult(result);
                    return response;
                }
            }

            return await HandleQuery(request, cancellationToken);
        }

        protected abstract Task<ResponseDto<TResponse>> HandleQuery(TRequest request, CancellationToken cancellationToken);

        protected void AddExceptionQueryResult(ResponseDto<TResponse> response, Exception exception)
        {
            var innerException = GetInnerException(exception);
            response.AddErrorResult(innerException!);
        }

        protected void AddExceptionQueryResult(ResponseDto<TResponse> response, Exception exception, string message)
        {
            var innerException = GetInnerException(exception);
            response.AddErrorResult(message, innerException!);
        }

        private static Exception? GetInnerException(Exception ex)
        {
            var limit = 10;
            var iteration = 0;
            var exception = ex;

            while (exception?.InnerException != null && iteration < limit)
            {
                exception = ex.InnerException;
                iteration++;
            }

            return exception;
        }
    }
}
