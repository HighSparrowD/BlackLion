using MyWebApi.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.HintEntities
{
    public class Hint
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public int ClassLocalisationId { get; set; }
        public Sections? Section { get; set; }
        public HintType Type { get; set; }
        public string Text { get; set; }
    }
}
