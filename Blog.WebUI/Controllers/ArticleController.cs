using Blog.Business.Dtos.ArticleDtos;
using Blog.Business.Services;
using Blog.WebUI.Extensions;
using Blog.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Blog.Business.Dtos.TagDtos;
using System.IO;
using System.Linq;

namespace Blog.WebUI.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly ILogger<ArticleController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ITagService _tagService;

        public ArticleController(IArticleService articleService, ILogger<ArticleController> logger, IWebHostEnvironment environment, ICommentService commentService, ITagService tagService)
        {
            _articleService = articleService;
            _logger = logger;
            _environment = environment;
            _commentService = commentService;
            _tagService = tagService;
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Create()
        {
            var tags = _tagService.GetAllTags();
            var viewModel = new AddArticleViewModel
            {
                AllTags = tags.Select(tag => new SelectListItem
                {
                    Value = tag.Id.ToString(),
                    Text = tag.Name
                }).ToList()
            };
            return View(viewModel);
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

            if (!User.IsLogged())
            {
                _logger.LogWarning("Kullanıcı kimliği doğrulanmadı.");
                return RedirectToAction("Register", "Auth");
            }

            var userId = User.GetUserId();
            if (userId == 0)
            {
                _logger.LogError("Kullanıcı kimlik bilgisi bulunamadı.");
                ViewBag.ErrorMessage = "Kullanıcı kimlik bilgisi bulunamadı.";
                return View(viewModel);
            }

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
                UserId = userId,
                TagIds = viewModel.SelectedTagIds 
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
                _logger.LogWarning($"Article with ID {id} not found.");
                return RedirectToAction("Index");
            }

            var tags = _tagService.GetAllTags();
            if (tags == null)
            {
                _logger.LogError("Etiketler alınamadı.");
                tags = new List<TagInfoDto>();
            }

            var selectedTagIds = article.TagIds ?? new List<int>();
            var viewModel = new AddArticleViewModel
            {
                Title = article.Title,
                Content = article.Content,
                ImageUrl = article.ImageUrl,
                SelectedTagIds = selectedTagIds,
                AllTags = tags.Select(tag => new SelectListItem
                {
                    Value = tag.Id.ToString(),
                    Text = tag.Name,
                    Selected = selectedTagIds.Contains(tag.Id)
                }).ToList()
            };

            ViewBag.ImagePath = article.ImageUrl;
            return View(viewModel);
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
                ImageUrl = newFileName,
                TagIds = viewModel.SelectedTagIds ?? new List<int>()
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
            var userId = User.GetUserId();
            var articles = User.IsAdmin()
                ? _articleService.GetAllArticles()
                : _articleService.GetArticlesByUserId(userId);

            return View(new ArticleViewModel { Articles = articles });
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int id)
        {
            var article = _articleService.GetArticleById(id);
            if (article == null)
            {
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

        public IActionResult Details(int id, string title)
        {
            var article = _articleService.GetArticleById(id);
            if (article == null)
            {
                return RedirectToAction("Index");
            }

            var expectedTitle = article.Title.Replace(" ", "-").Replace("'", "").ToLower();
            if (title != expectedTitle)
            {
                return RedirectToAction("Details", new { id, title = expectedTitle });
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
