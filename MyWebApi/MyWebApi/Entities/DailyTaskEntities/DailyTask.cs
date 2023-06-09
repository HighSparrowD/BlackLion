using WebApi.Enums;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace WebApi.Entities.DailyTaskEntities
{
    public class DailyTask
    {
        [Key]
        public long Id { get; set; }
        public int ClassLocalisationId { get; set; }
        public int Condition { get; set; }
        public string Description { get; set; }
        public int Reward { get; set; }
        public short RewardCurrency { get; set; }
        public int SectionId { get; set; }
        public short TaskType { get; set; }

        public async Task<string> GenerateAcquireMessage(DailyTask task)
        {
            var message = "";

            await Task.Run(() =>
            {
                var currency = "";

                if (task.RewardCurrency == (byte)SystemEnums.Currencies.Points)
                    currency = "p";
                else if (task.RewardCurrency == (byte)SystemEnums.Currencies.PersonalityPoints)
                    currency = " PERSONALITY points";

                message = $"You have accomplished a daily task!\n\n{task.Description}\n{task.Reward}{currency}";
            });

            return message;
        }
    }
}
