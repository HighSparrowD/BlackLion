using System.Collections.Generic;
using WebApi.Main.Enums.Tag;

namespace WebApi.Entities.UserActionEntities
{
    public class SimilarityBetweenUsers
    {
        public List<TagType> SimilarBy { get; set; }
        public int SimilarityCount { get; set; }
    }
}
