using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Blog.WebUI.Models
{
    public class AddArticleViewModel
    {
        [Display(Name = "Başlık")]
        [MaxLength(100, ErrorMessage = "Başlık en fazla 100 karakter uzunluğunda olabilir.")]
        [Required(ErrorMessage = "Başlık alanı boş bırakılamaz.")]
        public string Title { get; set; }

        [Display(Name = "İçerik")]
        [Required(ErrorMessage = "İçerik alanı boş bırakılamaz.")]
        public string Content { get; set; }

        [Display(Name = "Ürün Görseli")]
        public IFormFile? File { get; set; }

        public string? ImageUrl { get; set; }
    }
}
