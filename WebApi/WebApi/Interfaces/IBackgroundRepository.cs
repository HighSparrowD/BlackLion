using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Main.Models.User;

namespace WebApi.Interfaces
{
    public interface IBackgroundRepository
    {
        Task SaveBatchChanges(List<User> batch);
        Task RemoveStreakAsync();
        Task<List<User>> GetBatchToUpdate(int batchSize);
        Task DeleteOldTransactionsAsync();
        Task DeleteOldEncountersAsync();
        Task DeleteOldFeedbacksAsync();
        Task DeleteOldReportsAsync();
        Task DeleteOldRequestsAsync();
        Task DeleteOldUsersAsync();
        Task DeleteOldAdventuresWithReportsAsync();
    }
}
