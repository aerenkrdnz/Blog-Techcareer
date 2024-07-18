using Blog.Business.Dtos.HomeDtos;
using Blog.Business.Dtos.TagDtos;
using Blog.Business.Services;
using Blog.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Blog.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;
        private readonly ITagService _tagService;

        public HomeController(IHomeService homeService, ITagService tagService)
        {
            _homeService = homeService;
            _tagService = tagService;
        }

        public IActionResult Index(string searchTerm, List<int> selectedTagIds, int page = 1)
        {
            const int pageSize = 6;
            var articles = _homeService.GetAllArticles();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                articles = _homeService.SearchArticles(searchTerm);
            }

            if (selectedTagIds != null && selectedTagIds.Count > 0)
            {
                articles = _homeService.FilterArticlesByTags(selectedTagIds);
            }

            var pagedArticles = articles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalArticles = articles.Count;
            var totalPages = (int)Math.Ceiling(totalArticles / (double)pageSize);

            ViewBag.Tags = _tagService.GetAllTags() ?? new List<TagInfoDto>();

            var model = new HomeViewModel
            {
                Articles = pagedArticles,
                SearchTerm = searchTerm,
                Tags = ViewBag.Tags,
                CurrentPage = page,
                TotalPages = totalPages,
                SelectedTagIds = selectedTagIds ?? new List<int>()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Search(string searchTerm, int page = 1)
        {
            const int pageSize = 6;
            var articles = _homeService.SearchArticles(searchTerm);
            var pagedArticles = articles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalArticles = articles.Count;
            var totalPages = (int)Math.Ceiling(totalArticles / (double)pageSize);

            ViewBag.Tags = _tagService.GetAllTags() ?? new List<TagInfoDto>();

            var model = new HomeViewModel
            {
                Articles = pagedArticles,
                SearchTerm = searchTerm,
                Tags = ViewBag.Tags,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View("Index", model);
        }

        [HttpGet]
        public IActionResult FilterByTag(List<int> selectedTagIds, int page = 1)
        {
            const int pageSize = 6;
            var articles = _homeService.GetAllArticles();

            if (selectedTagIds != null && selectedTagIds.Count > 0)
            {
                articles = _homeService.FilterArticlesByTags(selectedTagIds);
            }

            var pagedArticles = articles
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalArticles = articles.Count;
            var totalPages = (int)Math.Ceiling(totalArticles / (double)pageSize);

            ViewBag.Tags = _tagService.GetAllTags() ?? new List<TagInfoDto>();

            var model = new HomeViewModel
            {
                Articles = pagedArticles,
                Tags = ViewBag.Tags,
                CurrentPage = page,
                TotalPages = totalPages,
                SelectedTagIds = selectedTagIds
            };

            return View("Index", model);
        }
    }
}
