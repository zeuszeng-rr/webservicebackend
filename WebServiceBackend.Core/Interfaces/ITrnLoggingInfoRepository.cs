using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServiceBackend.Core.Entities;

namespace WebServiceBackend.Core.Interfaces
{
    public interface ITrnLoggingInfoRepository
    {
        Task<TrnLoggingInfoEntity> AddAsync(TrnLoggingInfoEntity entity);
        Task<TrnLoggingInfoEntity> UpdateAsync(TrnLoggingInfoEntity entity);
    }
}
