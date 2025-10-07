using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServiceBackend.Core.Entities;
using WebServiceBackend.Core.Interfaces;
using WebServiceBackend.Infrastructure.Data;

namespace WebServiceBackend.Infrastructure.Repositories
{
    public class TrnLoggingInfoRepository : ITrnLoggingInfoRepository
    {
        private readonly AppDbContext _context;

        public TrnLoggingInfoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TrnLoggingInfoEntity> AddAsync(TrnLoggingInfoEntity entity)
        {
            _context.TrnLoggingInfoEntity.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TrnLoggingInfoEntity> UpdateAsync(TrnLoggingInfoEntity entity)
        {
            _context.TrnLoggingInfoEntity.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
