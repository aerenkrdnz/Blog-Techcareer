using Blog.Business.Dtos.ArticleDtos;
using Blog.Business.Services;
using Blog.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Blog.WebUI.Controllers
{

    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly ILogger<ArticleController> _logger;
        private readonly IWebHostEnvironment _environment;

        public ArticleController(IArticleService articleService, ILogger<ArticleController> logger, IWebHostEnvironment environment, ICommentService commentService)
        {
            _articleService = articleService;
            _logger = logger;
            _environment = environment;
            _commentService = commentService;
        }
        [Authorize(Roles = "User,Admin")]
        public IActionResult Create()
        {
            return View(new AddArticleViewModel());
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Create(AddArticleViewModel viewModel)
        {
            _logger.LogInformation("Oluşturma işlemi çağrıldı.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model durumu geçerli değil.");
                return View(viewModel);
            }

            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Kullanıcı kimliği doğrulanmadı.");
                return RedirectToAction("Register", "Auth");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                _logger.LogError("Kullanıcı kimlik bilgisi bulunamadı. Mevcut kimlikler: " + string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}")));
                ViewBag.ErrorMessage = "Kullanıcı kimlik bilgisi bulunamadı.";
                return View(viewModel);
            }

            var userId = int.Parse(userIdClaim.Value);
            _logger.LogInformation($"Kullanıcı ID: {userId}");

            var newFileName = "no-images.png";

            if (viewModel.File is not null)
            {
                var allowedFileTypes = new string[] { "image/jpeg", "image/jpg", "image/png", "image/jfif" };
                var allowedFileExtensions = new string[] { ".jpg", ".jpeg", ".png", ".jfif" };
                var fileContentType = viewModel.File.ContentType;
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(viewModel.File.FileName);
                var fileExtension = Path.GetExtension(viewModel.File.FileName);

                if (!allowedFileTypes.Contains(fileContentType) || !allowedFileExtensions.Contains(fileExtension))
                {
                    ViewBag.ErrorMessage = "Desteklenmeyen dosya türü.";
                    return View(viewModel);
                }

                newFileName = fileNameWithoutExtension + "-" + Guid.NewGuid() + fileExtension;
                var folderPath = Path.Combine("images", "articles");

                var wwwrootFolderPath = Path.Combine(_environment.WebRootPath, folderPath);
                var wwwrootFilePath = Path.Combine(wwwrootFolderPath, newFileName);

                Directory.CreateDirectory(wwwrootFolderPath);

                using (var filestream = new FileStream(wwwrootFilePath, FileMode.Create))
                {
                    viewModel.File.CopyTo(filestream);
                }
            }

            var formData = new AddArticleDto
            {
                Title = viewModel.Title,
                Content = viewModel.Content,
                ImageUrl = newFileName,
                UserId = userId
            };

            var result = _articleService.AddArticle(formData);
            _logger.LogInformation($"Makale ekleme sonucu: {result.IsSucceed}");

            if (result.IsSucceed)
            {
                _logger.LogInformation("Makale başarıyla oluşturuldu.");
                return RedirectToAction("Index", "Article");
            }
            else
            {
                _logger.LogError($"Makale oluşturma hatası: {result.Message}");
                ViewBag.ErrorMessage = result.Message;
                return View(viewModel);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var article = _articleService.GetArticleById(id);
            if (article == null)
            {
                return RedirectToAction("Index");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (!User.IsInRole("Admin") && article.UserId != userId)
            {
                ViewBag.ErrorMessage = "Bu makaleyi düzenleme yetkiniz yok.";
                return RedirectToAction("Index");
            }

            var editDto = new AddArticleViewModel()
            {
                Title = article.Title,
                Content = article.Content,
                ImageUrl = article.ImageUrl
            };

            ViewBag.ImagePath = article.ImageUrl;
            return View(editDto);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult Edit(AddArticleViewModel viewModel, int id)
        {
            var article = _articleService.GetArticleById(id);
            if (article == null)
            {
                return RedirectToAction("Index");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (!User.IsInRole("Admin") && article.UserId != userId)
            {
                ViewBag.ErrorMessage = "Bu makaleyi düzenleme yetkiniz yok.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var newFileName = viewModel.ImageUrl ?? "no-images.png";

            if (viewModel.File is not null)
            {
                var allowedFileTypes = new string[] { "image/jpeg", "image/jpg", "image/png", "image/jfif" };
                var allowedFileExtensions = new string[] { ".jpg", ".jpeg", ".png", ".jfif" };
                var fileContentType = viewModel.File.ContentType;
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(viewModel.File.FileName);
                var fileExtension = Path.GetExtension(viewModel.File.FileName);

                if (!allowedFileTypes.Contains(fileContentType) || !allowedFileExtensions.Contains(fileExtension))
                {
                    ViewBag.ErrorMessage = "Desteklenmeyen dosya türü.";
                    return View(viewModel);
                }

                newFileName = fileNameWithoutExtension + "-" + Guid.NewGuid() + fileExtension;
                var folderPath = Path.Combine("images", "articles");

                var wwwrootFolderPath = Path.Combine(_environment.WebRootPath, folderPath);
                var wwwrootFilePath = Path.Combine(wwwrootFolderPath, newFileName);

                Directory.CreateDirectory(wwwrootFolderPath);

                using (var filestream = new FileStream(wwwrootFilePath, FileMode.Create))
                {
                    viewModel.File.CopyTo(filestream);
                }
            }

            var formData = new UpdateArticleDto
            {
                Id = id,
                Title = viewModel.Title,
                Content = viewModel.Content,
                ImageUrl = newFileName
            };

            var result = _articleService.UpdateArticle(formData, id);

            if (result.IsSucceed)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorMessage = result.Message;
                return View(viewModel);
            }
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var articles = User.IsInRole("Admin")
                ? _articleService.GetAllArticles()
                : _articleService.GetArticlesByUserId(userId);

            return View(articles);
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int id)
        {
            var article = _articleService.GetArticleById(id);
            if (article == null)
            {
                return RedirectToAction("Index");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (!User.IsInRole("Admin") && article.UserId != userId)
            {
                ViewBag.ErrorMessage = "Bu makaleyi silme yetkiniz yok.";
                return RedirectToAction("Index");
            }

            var result = _articleService.DeleteArticle(id);

            if (result.IsSucceed)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorMessage = result.Message;
                return RedirectToAction("Index");
            }
        }

       
        public IActionResult Details(int id)
        {
            var article = _articleService.GetArticleById(id);
            if (article == null)
            {
                return RedirectToAction("Index");
            }

            var comments = _commentService.GetCommentsByArticleId(id);
            ViewBag.Comments = comments;

            if (string.IsNullOrEmpty(article.ImageUrl))
            {
                article.ImageUrl = "no-images.png";
            }

            return View(article);
        }

    }
}
