using AutoMapper;
using __NAMESPACE__.Dto.Base;
using __NAMESPACE__.Repository.Abstractions.Transactions;
using MediatR;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CA2208 // Instantiate argument exceptions correctly
namespace __NAMESPACE__.Domain.Commands.Base
{
    public abstract class CommandHandlerBaseBase<TRequest, TResponse>(IUnitOfWork unitOfWork) : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : ResponseDto, new()
    {
        protected virtual bool UseTransaction => true;
        protected virtual int ConcurrencyAttempts => 3;
        protected virtual bool ConcurrencyThrowException => false;

        protected readonly IMapper? _mapper;
        protected readonly IMediator? _mediator;
        protected readonly IUnitOfWork? _unitOfWork = unitOfWork;
        protected readonly CommandValidatorBase<TRequest>? _validator;

        public CommandHandlerBaseBase(IUnitOfWork unitOfWork, IMapper mapper) : this(unitOfWork)
            => _mapper = mapper;

        public CommandHandlerBaseBase(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator) : this(unitOfWork, mapper)
            => _mediator = mediator;

        public CommandHandlerBaseBase(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator, CommandValidatorBase<TRequest> validator) : this(unitOfWork, mapper, mediator)
            => _validator = validator;

        public CommandHandlerBaseBase(IUnitOfWork unitOfWork, IMapper mapper, CommandValidatorBase<TRequest> validator) : this(unitOfWork, mapper, null!, validator)
            => _validator = validator;

        public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var attempts = 0;

            while (attempts < ConcurrencyAttempts)
            {
                try
                {
                    return await HandleAttempt(request, cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    attempts++;
                    if (ConcurrencyThrowException || attempts == ConcurrencyAttempts) throw;
                    continue;
                }
                catch (ResultException<TResponse> rex)
                {
                    return rex.Result;
                }
                catch (Exception ex)
                {
                    var exception = (ex.InnerException?.InnerException ?? ex.InnerException) ?? ex;
                    if (exception.Message.Contains(Resources.Common.DeleteReferenceError))
                    {
                        var response = new TResponse();
                        response.AddErrorResult(Resources.Common.DeleteReferenceMessage);
                        return response;
                    }

                    var errorResponse = await OnHandleError(ex);
                    if (errorResponse != default(TResponse)) return errorResponse;
                    throw;
                }
            }

            return null!;
        }

        private async Task<TResponse> HandleAttempt(TRequest request, CancellationToken cancellationToken)
        {
            if (UseTransaction)
            {
                if (_unitOfWork == null)
                    throw new ArgumentNullException(nameof(_unitOfWork));

                var responseTransaction = await _unitOfWork.ExecuteInTransactionAsync(request, async (requestParam, cancellationTokenParam) =>
                {
                    return await ValidateAndHandle(requestParam, cancellationTokenParam);
                }, null!, cancellationToken);

                if (responseTransaction.IsValid) _unitOfWork.SendAudit();

                return responseTransaction;
            }

            if (_unitOfWork == null)
                throw new ArgumentNullException(nameof(_unitOfWork));

            var response = await _unitOfWork.ExecuteAsync(request, async (requestParam, cancellationTokenParam) =>
            {
                return await ValidateAndHandle(requestParam, cancellationTokenParam);
            }, null!, cancellationToken);

            if (response.IsValid) _unitOfWork.SendAudit();

            return response;
        }

        private async Task<TResponse> ValidateAndHandle(TRequest request, CancellationToken cancellationToken)
        {
            if (_validator != null && _validator.Enabled)
            {
                var result = await _validator.ValidateAsync(request, cancellationToken);

                if (!result.IsValid)
                {
                    var response = new TResponse();
                    response.AddErrorResult(result);
                    throw new ResultException<TResponse>(response);
                }
            }

            var resultCommand = await HandleCommand(request, cancellationToken);
            if (!resultCommand.IsValid)
                throw new ResultException<TResponse>(resultCommand);

            return resultCommand;
        }

        public virtual async Task<TResponse?> OnHandleError(Exception ex)
            => await Task.FromResult(default(TResponse));

        public abstract Task<TResponse> HandleCommand(TRequest request, CancellationToken cancellationToken);

        protected void AddExceptionCommandResult(ResponseDto response, Exception exception)
        {
            var innerException = GetInnerException(exception);
            response.AddErrorResult(innerException!);
        }

        protected void AddExceptionCommandResult(ResponseDto response, Exception exception, string message)
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

    public abstract class CommandHandlerBase<TRequest> : CommandHandlerBaseBase<TRequest, ResponseDto> where TRequest : CommandBase
    {
        protected CommandHandlerBase(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        protected CommandHandlerBase(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        protected CommandHandlerBase(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator) : base(unitOfWork, mapper, mediator) { }

        protected CommandHandlerBase(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator, CommandValidatorBase<TRequest> validator) : base(unitOfWork, mapper, mediator, validator) { }

        protected CommandHandlerBase(IUnitOfWork unitOfWork, IMapper mapper, CommandValidatorBase<TRequest> validator) : base(unitOfWork, mapper, null!, validator) { }

        public override async Task<ResponseDto> Handle(TRequest request, CancellationToken cancellationToken)
        {
            if (_validator != null && !request.Validate) _validator.Disable();

            var response = await base.Handle(request, cancellationToken);

            _validator?.Enable();

            return response;
        }
    }

    public abstract class CommandHandlerBase<TRequest, TResponse> : CommandHandlerBaseBase<TRequest, ResponseDto<TResponse>> where TRequest : CommandBase<TResponse>
    {
        protected CommandHandlerBase(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        protected CommandHandlerBase(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper) { }

        protected CommandHandlerBase(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator) : base(unitOfWork, mapper, mediator) { }

        protected CommandHandlerBase(IUnitOfWork unitOfWork, IMapper mapper, CommandValidatorBase<TRequest> validator) : base(unitOfWork, mapper, null!, validator) { }

        protected CommandHandlerBase(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator, CommandValidatorBase<TRequest> validator) : base(unitOfWork, mapper, mediator, validator) { }

        public override async Task<ResponseDto<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
        {
            if (_validator != null && !request.Validate) _validator.Disable();

            var response = await base.Handle(request, cancellationToken);

            _validator?.Enable();

            return response;
        }
    }
}
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
