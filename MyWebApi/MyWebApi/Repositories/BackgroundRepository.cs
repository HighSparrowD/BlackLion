using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Entities.UserInfoEntities;
using WebApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Repositories
{
    public class BackgroundRepository : IBackgroundRepository
    {
        private readonly UserContext _context;
        private const int _oldTransactionSpan = 30;
        private const int _oldEncountersSpan = 3;

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

        public async Task SaveBatchChanges(List<User> batch)
        {
            _context.UpdateRange(batch);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOldTransactionsAsync()
        {
            var sql = $"DELETE FROM \"transactions\" WHERE EXTRACT(DAY FROM (NOW() - \"PointInTime\")) >= {_oldTransactionSpan}";
            await _context.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task DeleteOldEncountersAsync()
        {
            var sql = $"DELETE FROM \"encounters\" WHERE EXTRACT(DAY FROM (NOW() - \"EncounterDate\")) >= {_oldEncountersSpan}";
            await _context.Database.ExecuteSqlRawAsync(sql);
        }

    }
}
