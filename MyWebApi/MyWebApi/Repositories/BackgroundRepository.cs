using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Entities.UserInfoEntities;
using WebApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace WebApi.Repositories
{
    public class BackgroundRepository : IBackgroundRepository
    {
        private readonly UserContext _context;
        private const int _oldTransactionSpan = 0;

        public BackgroundRepository(UserContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetBatchToUpdate(int batchSize)
        {
            return await _context.Users.Where(u => !u.IsUpdated)
                .Take(batchSize)
                .Include(u => u.Settings)
                .ToListAsync(); ;
        }

        public async Task DeleteOldTransactionsAsync()
        {
            var now = DateTime.UtcNow;
            var transactions = await _context.Transaction.Where(t => (now - t.PointInTime).Days >= _oldTransactionSpan)
                .ToListAsync();

            _context.Transaction.RemoveRange(transactions);
            await _context.SaveChangesAsync();
        }

        public async Task SaveBatchChanges(List<User> batch)
        {
            _context.UpdateRange(batch);
            await _context.SaveChangesAsync();
        }
    }
}
