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
    public record UpdateTrnLoggingInfoCommand(TrnLoggingInfoEntity trnLoggingInfoEntity) : IRequest<TrnLoggingInfoEntity>;
    public class UpdateTrnLoggingInfoCommandHandler : IRequestHandler<UpdateTrnLoggingInfoCommand,TrnLoggingInfoEntity>
    {
        private readonly ITrnLoggingInfoRepository _rnLoggingInfoRepository;
        public UpdateTrnLoggingInfoCommandHandler(ITrnLoggingInfoRepository repository)
        { 
            _rnLoggingInfoRepository = repository;
        }
        public async Task<TrnLoggingInfoEntity> Handle(UpdateTrnLoggingInfoCommand request, CancellationToken cancellationToken)
        {
            var entity = request.trnLoggingInfoEntity;
            return await _rnLoggingInfoRepository.UpdateAsync(entity);
        }
    }
}
