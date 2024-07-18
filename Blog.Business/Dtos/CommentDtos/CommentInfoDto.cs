namespace Blog.Business.Dtos.CommentDtos
{
    public class CommentInfoDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public int ArticleId { get; set; }
        public int UserId { get; set; }
        public string ProfileImageUrl { get; set; }

    }
}
