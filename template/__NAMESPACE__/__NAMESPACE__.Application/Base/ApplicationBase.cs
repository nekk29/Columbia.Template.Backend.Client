using AutoMapper;
using __NAMESPACE__.Repository.Abstractions.Transactions;
using MediatR;

namespace __NAMESPACE__.Application.Base
{
    public class ApplicationBase(IMediator mediator)
    {
        protected readonly IMapper? _mapper;
        protected readonly IMediator _mediator = mediator;
        protected readonly IUnitOfWork? _unitOfWork;

        public ApplicationBase(IMediator mediator, IMapper mapper) : this(mediator)
            => _mapper = mapper;

        public ApplicationBase(IMediator mediator, IMapper mapper, IUnitOfWork unitOfWork) : this(mediator, mapper)
            => _unitOfWork = unitOfWork;
    }
}
