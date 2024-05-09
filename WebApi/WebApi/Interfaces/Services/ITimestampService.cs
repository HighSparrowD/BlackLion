using System;

namespace WebApi.Interfaces.Services
{
    public interface ITimestampService
    {
        void SetTimestamp(DateTime timestamp);
        
        void SetTimestampUtc(DateTime timestamp);

        void ResetTimestamp();

        void ResetTimestampUtc();

        DateTime GetNow();

        DateTime GetUtcNow();
    }
}
