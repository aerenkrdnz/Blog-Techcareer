using Blog.Business.Dtos.UserDtos;
using Blog.Business.Services;
using Blog.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.WebUI.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;

        public UserController(IUserService userService, IWebHostEnvironment environment)
        {
            _userService = userService;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = _userService.GetUserById(userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (model.Id != loggedInUserId)
            {
                ViewBag.ErrorMessage = "Kullanıcı bilgileri uyuşmuyor.";
                return View(model);
            }

            var newFileName = model.ProfileImageUrl;

            if (model.ProfileImage != null)
            {
                var allowedFileTypes = new string[] { "image/jpeg", "image/jpg", "image/png", "image/jfif" };
                var allowedFileExtensions = new string[] { ".jpg", ".jpeg", ".png", ".jfif" };
                var fileContentType = model.ProfileImage.ContentType;
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(model.ProfileImage.FileName);
                var fileExtension = Path.GetExtension(model.ProfileImage.FileName);

                if (!allowedFileTypes.Contains(fileContentType) || !allowedFileExtensions.Contains(fileExtension))
                {
                    ViewBag.ErrorMessage = "Desteklenmeyen dosya türü.";
                    return View(model);
                }

                newFileName = fileNameWithoutExtension + "-" + Guid.NewGuid() + fileExtension;
                var folderPath = Path.Combine("images", "profiles");

                var wwwrootFolderPath = Path.Combine(_environment.WebRootPath, folderPath);
                var wwwrootFilePath = Path.Combine(wwwrootFolderPath, newFileName);

                Directory.CreateDirectory(wwwrootFolderPath);

                using (var filestream = new FileStream(wwwrootFilePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(filestream);
                }
            }

            var updateUserDto = new UpdateUserDto
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                ProfileImageUrl = newFileName
            };

            var result = _userService.UpdateUser(updateUserDto);

            if (result.IsSucceed)
            {
                return RedirectToAction("Profile");
            }
            else
            {
                ViewBag.ErrorMessage = result.Message;
                return View(model);
            }
        }


    }
}
