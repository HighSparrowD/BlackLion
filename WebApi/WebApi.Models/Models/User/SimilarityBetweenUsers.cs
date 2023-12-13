using WebApi.Enums.Enums.Tag;

namespace WebApi.Models.Models.User
{
    public class SimilarityBetweenUsers
    {
        public List<TagType> SimilarBy { get; set; }
        public int SimilarityCount { get; set; }
    }
}
