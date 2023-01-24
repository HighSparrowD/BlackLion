using MyWebApi.Enums;
using System.Collections.Generic;

namespace MyWebApi.Entities.UserActionEntities
{
    public class SimilarityBetweenUsers
    {
        public List<TagType> SimilarBy { get; set; }
        public int SimilarityCount { get; set; }
    }
}
