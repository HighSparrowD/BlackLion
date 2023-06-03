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

        public BackgroundRepository(UserContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetBatchToUpdate(int batchSize)
        {
            return await _context.users.Where(u => !u.IsUpdated).Take(batchSize)
                .ToListAsync(); ;
        }

        public async Task SaveBatchChanges(List<User> batch)
        {
            _context.UpdateRange(batch);
            await _context.SaveChangesAsync();
        }
    }
}
