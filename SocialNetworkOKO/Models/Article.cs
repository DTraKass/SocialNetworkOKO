namespace SocialNetworkOKO.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Author { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<ArticleTag>? ArticleTags { get; set; } = new List<ArticleTag>();
        public virtual ICollection<Article>? RelatedArticles { get; set; }
    }
}
