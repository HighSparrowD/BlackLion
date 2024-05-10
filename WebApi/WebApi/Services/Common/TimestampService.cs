using System;
using WebApi.Interfaces.Services;

namespace WebApi.Services.Common
{
    public class TimestampService : ITimestampService
    {
        private DateTime? timestamp;
        private DateTime? timestampUtc;

        public void SetTimestamp(DateTime timestamp)
        {
            timestampUtc = timestamp;
        }

        public void SetTimestampUtc(DateTime timestamp)
        {
            timestampUtc = timestamp;
        }

        public void ResetTimestamp()
        {
            timestamp = null;
        }

        public void ResetTimestampUtc()
        {
            timestampUtc = null;
        }

        public DateTime GetNow()
        {
            if (timestamp != null)
                return timestamp.Value;

            return DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
        }

        public DateTime GetUtcNow()
        {
            if (timestampUtc != null)
                return timestampUtc.Value;

            return DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
        }
    }
}
