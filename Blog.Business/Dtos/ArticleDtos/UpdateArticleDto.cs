namespace Blog.Business.Dtos.ArticleDtos
{
    public class UpdateArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public List<int> TagIds { get; set; } = new List<int>();
    }
}
