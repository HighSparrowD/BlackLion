using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using WebApi.Main.Entities.User;
using WebApi.Interfaces.Services;

namespace WebApi.Repositories
{
    public class BackgroundRepository : IBackgroundRepository
    {
        private readonly UserContext _context;
        private readonly ITimestampService timestamp;

        private const short _oldTransactionSpan = 30;
        private const short _oldEncountersSpan = 3;
        private const short _oldFeedbacksSpan = 30;
        private const short _oldReportsSpan = 30;
        private const short _oldRequestsSpan = 2;
        private const short _oldUsersSpan = 0;
        private const short _oldAdventuresSpan = 15;

        public BackgroundRepository(UserContext context, ITimestampService timestampService)
        {
            _context = context;
            timestamp = timestampService;
        }

        public async Task<List<User>> GetBatchToUpdate(int batchSize)
        {
            return await _context.Users.Where(u => !u.IsUpdated)
                .Take(batchSize)
                .Include(u => u.Settings)
                .Include(u => u.Statistics)
                .ToListAsync(); ;
        }

        public async Task RemoveStreakAsync()
        {
            var sql = $"UPDATE user_statistics SET \"UseStreak\" = 0 WHERE \"UserId\" IN (SELECT \"Id\" FROM users WHERE \"IsUpdated\" = True);";
            await _context.Database.ExecuteSqlRawAsync(sql);
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

        public async Task DeleteOldFeedbacksAsync()
        {
            var sql = $"DELETE FROM \"feedbacks\" WHERE EXTRACT(DAY FROM (NOW() - \"InsertedUtc\")) >= {_oldFeedbacksSpan} AND \"AdminId\" IS NOT NULL";
            await _context.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task DeleteOldReportsAsync()
        {
            var sql = $"DELETE FROM \"user_reports\" WHERE EXTRACT(DAY FROM (NOW() - \"InsertedUtc\")) >= {_oldReportsSpan} AND \"AdminId\" IS NOT NULL";
            await _context.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task DeleteOldRequestsAsync()
        {
            var sql = $"DELETE FROM \"requests\" WHERE EXTRACT(DAY FROM (NOW() - \"AnsweredTimeStamp\")) >= {_oldRequestsSpan} AND \"Answer\" IS NOT NULL";
            await _context.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task DeleteOldUsersAsync()
        {
            var oldUserIds = await _context.Users.Where(u => (u.DeleteDate - timestamp.GetUtcNow()).Value.Days >= _oldUsersSpan)
                .Select(u => u.Id)
                .ToListAsync();

            if (oldUserIds.Count == 0)
                return;

            var formattedIds = string.Join(", ", oldUserIds);

            var sql = $"DELETE FROM \"users\" WHERE EXTRACT(DAY FROM (NOW() - \"DeleteDate\")) >= {_oldUsersSpan};";
            sql += $"DELETE FROM \"user_data\" WHERE \"Id\" IN ({formattedIds});";
            sql += $"DELETE FROM \"user_statistics\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"user_settings\" WHERE \"Id\" IN ({formattedIds});";
            sql += $"DELETE FROM \"active_effects\" WHERE \"UserId\" IN ({formattedIds});";

            //sql += $"DELETE FROM \"ads\" WHERE \"Id\" IN ({formattedIds});";

            sql += $"DELETE FROM \"adventure_attendees\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"adventure_templates\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"adventures\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"balances\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"black_lists\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"black_lists\" WHERE \"BannedUserId\" IN ({formattedIds});";

            //sql += $"DELETE FROM \"daily_rewards\" WHERE \"UserId\" IN ({formattedIds});";

            sql += $"DELETE FROM \"encounters\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"feedbacks\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"invitation_credentials\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"invitations\" WHERE \"InvitedUserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"notifications\" WHERE \"UserId\" IN ({formattedIds});";

            //sql += $"DELETE FROM \"personality_points\" WHERE \"UserId\" IN ({formattedIds});";
            //sql += $"DELETE FROM \"personality_stats\" WHERE \"UserId\" IN ({formattedIds});";

            //sql += $"DELETE FROM \"sponsor_contact_info\" WHERE \"SponsorId\" IN ({formattedIds});";
            //sql += $"DELETE FROM \"sponsor_languages\" WHERE \"SponsorId\" IN ({formattedIds});";
            //sql += $"DELETE FROM \"sponsor_notifications\" WHERE \"SponsorId\" IN ({formattedIds});";
            //sql += $"DELETE FROM \"sponsor_ratings\" WHERE \"SponsorId\" IN ({formattedIds});";
            //sql += $"DELETE FROM \"sponsor_stats\" WHERE \"SponsorId\" IN ({formattedIds});";
            //sql += $"DELETE FROM \"sponsors\" WHERE \"Id\" IN ({formattedIds});";

            sql += $"DELETE FROM \"verification_requests\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"transactions\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"trust_levels\" WHERE \"Id\" IN ({formattedIds});";
            sql += $"DELETE FROM \"user_achievements\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"user_locations\" WHERE \"Id\" IN ({formattedIds});";
            sql += $"DELETE FROM \"user_reports\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"user_reports\" WHERE \"SenderId\" IN ({formattedIds});";
            //TODO: Fix related error
            //sql += $"DELETE FROM \"user_reports\" WHERE \"AdventureId\" IN (SELECT \"Id\" FROM \"adventures\" WHERE \"UserId\" IN ({string.Join(",", oldUserIds)}))";
            sql += $"DELETE FROM \"user_tags\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"user_tests\" WHERE \"UserId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"user_visits\" WHERE \"UserId\" IN ({formattedIds});";

            await _context.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task DeleteOldAdventuresWithReportsAsync()
        {
            var oldAdventureIds = await _context.Adventures.Where(a => (a.DeleteDate - timestamp.GetUtcNow()).Value.Days >= _oldAdventuresSpan)
                .Select(u => u.Id)
                .ToListAsync();

            if (oldAdventureIds.Count == 0)
                return;

            var formattedIds = string.Join(", ", oldAdventureIds);

            var sql = $"DELETE FROM \"user_reports\" WHERE \"AdventureId\" IN ({formattedIds});";
            sql += $"DELETE FROM \"adventures\" WHERE EXTRACT(DAY FROM (NOW() - \"DeleteDate\")) >= {_oldAdventuresSpan}";

            await _context.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
