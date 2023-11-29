using WebApi.Enums;
using System.Collections.Generic;

namespace WebApi.Entities.UserActionEntities
{
    public class SimilarityBetweenUsers
    {
        public List<TagType> SimilarBy { get; set; }
        public int SimilarityCount { get; set; }
    }
}
