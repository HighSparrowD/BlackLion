﻿using WebApi.Entities.UserInfoEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Interfaces
{
    public interface IBackgroundRepository
    {
        Task SaveBatchChanges(List<User> batch);
        Task<List<User>> GetBatchToUpdate(int batchSize);
    }
}