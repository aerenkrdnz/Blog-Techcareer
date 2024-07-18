using Blog.Business.Dtos.UserDtos;
using Blog.Business.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Blog.WebUI.Models;

namespace Blog.WebUI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("Auth/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet]
        [Route("KayitOl")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("KayitOl")]
        public IActionResult Register(RegisterViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return View(formData);
            }
            var addUserDto = new AddUserDto()
            {
                Email = formData.Email.Trim(),
                FirstName = formData.FirstName.Trim(),
                LastName = formData.LastName.Trim(),
                Password = formData.Password.Trim(),
            };
            var result = _userService.AddUser(addUserDto);
            if (result.IsSucceed)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = result.Message;
                return View(formData);
            }


        }
        public async Task<IActionResult> Login(LoginViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }

            var loginDto = new LoginDto()
            {
                Email = formData.Email,
                Password = formData.Password
            };

            var userInfo = _userService.LoginUser(loginDto);
            if (userInfo is null)
            {
                return RedirectToAction("Index", "Home");
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()), 
        new Claim("id", userInfo.Id.ToString()),
        new Claim("email", userInfo.Email),
        new Claim("firstName", userInfo.FirstName),
        new Claim("lastName", userInfo.LastName),
        new Claim("userType", userInfo.UserType.ToString()),
        new Claim(ClaimTypes.Role, userInfo.UserType.ToString())
    };

            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(99)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), authProperties);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
