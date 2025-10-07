using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServiceBackend.Core.Entities;
using WebServiceBackend.Core.Interfaces;

namespace WebServiceBackend.Application.Commands
{
    public  record AddTrnLoggingInfoCommand(TrnLoggingInfoEntity trnLoggingInfo) : IRequest<TrnLoggingInfoEntity>;
    public class AddTrnLoggingInfoCommandHandler : IRequestHandler<AddTrnLoggingInfoCommand,TrnLoggingInfoEntity>
    {
        private readonly ITrnLoggingInfoRepository _repository;
        public AddTrnLoggingInfoCommandHandler(ITrnLoggingInfoRepository repository)
        {
            _repository = repository;
        }
        public async Task<TrnLoggingInfoEntity> Handle(AddTrnLoggingInfoCommand request, CancellationToken cancellationToken)
        { 
            var result = await _repository.AddAsync(request.trnLoggingInfo);
            return result;
        }
    }
}
