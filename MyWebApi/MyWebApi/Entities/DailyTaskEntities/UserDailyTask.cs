using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.DailyTaskEntities
{
    public class UserDailyTask
    {
        [Key]
        public long UserId { get; set; }
        [Key]
        public long DailyTaskId { get; set; }
        public int DailyTaskClassLocalisationId { get; set; }
        public int Progress { get; set; }
        public string AcquireMessage { get; set; }
        public bool IsAcquired{ get; set; }
        public virtual DailyTask DailyTask{ get; set; }
    }
}
