
namespace SocialNetworkOKO.Models
{
    public class ArticleTag
    {
        public Guid Id { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
