using Blog.Business.Dtos.TagDtos;
using Blog.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TagController : Controller
    {
        private readonly ITagService _tagService;
        private readonly ILogger<TagController> _logger;

        public TagController(ITagService tagService, ILogger<TagController> logger)
        {
            _tagService = tagService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var tags = _tagService.GetAllTags();
            return View(tags);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(AddTagDto model)
        {
            if (ModelState.IsValid)
            {
                var result = _tagService.AddTag(model);
                if (result.IsSucceed)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.ErrorMessage = result.Message;
            }
            return View(model);
        }
    }
}
