using System.ComponentModel.DataAnnotations;

namespace Blog.WebUI.Models
{
    public class AddCommentViewModel
    {
        [Required(ErrorMessage = "Yorum içeriği boş bırakılamaz.")]
        [MaxLength(500, ErrorMessage = "Yorum en fazla 500 karakter uzunluğunda olabilir.")]
        public string Content { get; set; }

        public int ArticleId { get; set; }
    }
}
