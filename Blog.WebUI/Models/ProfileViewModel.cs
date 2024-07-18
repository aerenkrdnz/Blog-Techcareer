using System.ComponentModel.DataAnnotations;

namespace Blog.WebUI.Models
{
    public class ProfileViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Adı")]
        [MaxLength(25, ErrorMessage = "İsim maksimum 25 karakter uzunluğunda olabilir.")]
        [Required(ErrorMessage = "Ad alanı boş bırakılamaz.")]
        public string FirstName { get; set; }

        [Display(Name = "Soyadı")]
        [MaxLength(25, ErrorMessage = "Soyad en fazla 25 karakter uzunluğunda olabilir.")]
        [Required(ErrorMessage = "Soyad alanı boş bırakılamaz.")]
        public string LastName { get; set; }

        [Display(Name = "Eposta")]
        [Required(ErrorMessage = "Eposta alanı boş bırakılamaz.")]
        public string Email { get; set; }

        [Display(Name = "Profil Resmi")]
        public IFormFile? ProfileImage { get; set; }

        public string? ProfileImageUrl { get; set; }
    }
}
