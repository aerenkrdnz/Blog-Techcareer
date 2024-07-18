using Blog.Business.Services;
using Blog.Business.Dtos.CommentDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Blog.WebUI.Models;

namespace Blog.WebUI.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IArticleService _articleService;
        private readonly ILogger<CommentController> _logger;

        public CommentController(ICommentService commentService, IArticleService articleService, ILogger<CommentController> logger)
        {
            _commentService = commentService;
            _articleService = articleService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Add(AddCommentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Article", new { id = viewModel.ArticleId });
            }

            var article = _articleService.GetArticleById(viewModel.ArticleId);
            if (article == null)
            {
                ViewBag.ErrorMessage = "Makale bulunamadı.";
                return RedirectToAction("Details", "Article", new { id = viewModel.ArticleId });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = int.Parse(userIdClaim.Value);

            var addCommentDto = new AddCommentDto
            {
                Content = viewModel.Content,
                ArticleId = viewModel.ArticleId,
                UserId = userId
            };

            var result = _commentService.AddComment(addCommentDto);

            if (result.IsSucceed)
            {
                return RedirectToAction("Details", "Article", new { id = viewModel.ArticleId });
            }
            else
            {
                ViewBag.ErrorMessage = result.Message;
                return RedirectToAction("Details", "Article", new { id = viewModel.ArticleId });
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = int.Parse(userIdClaim.Value);
            var comment = _commentService.GetCommentById(id);
            if (comment == null)
            {
                ViewBag.ErrorMessage = "Yorum bulunamadı.";
                return RedirectToAction("Index", "Article");
            }

            if (!User.IsInRole("Admin") && comment.UserId != userId)
            {
                ViewBag.ErrorMessage = "Yorumu silme yetkiniz yok.";
                return RedirectToAction("Details", "Article", new { id = comment.ArticleId });
            }

            var result = _commentService.DeleteComment(id);

            if (result.IsSucceed)
            {
                return RedirectToAction("Details", "Article", new { id = comment.ArticleId });
            }
            else
            {
                ViewBag.ErrorMessage = result.Message;
                return RedirectToAction("Details", "Article", new { id = comment.ArticleId });
            }
        }
    }
}
