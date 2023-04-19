using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyWebApi.Data;
using MyWebApi.Entities.UserInfoEntities;
using MyWebApi.Interfaces;
using MyWebApi.Migrations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebApi.Repositories
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
            return await _context.SYSTEM_USERS.Where(u => !u.IsUpdated).Take(batchSize)
                .ToListAsync(); ;
        }

        public async Task SaveBatchChanges(List<User> batch)
        {
            _context.UpdateRange(batch);
            await _context.SaveChangesAsync();
        }
    }
}
