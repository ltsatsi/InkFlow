using DomainLayer.Dtos.Account;
using DomainLayer.Mappers;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }




        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new SignUpDto());
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDto signUpDto)
        {
            if (!ModelState.IsValid)
                return View(signUpDto);

            var user = signUpDto.ToUser();
            var result = await _userManager.CreateAsync(user, signUpDto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(signUpDto);
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, "User");

            if (!addToRoleResult.Succeeded)
            {
                ViewBag.AddToRoleError = $"User {signUpDto.Email} created but failed to assign role.";
                return View(signUpDto);
            }

            return RedirectToAction(nameof(SignIn));
        }




        [HttpGet]
        public IActionResult SignIn(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new SignInDto());
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInDto signInDto, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(signInDto);

            var result = await _signInManager.PasswordSignInAsync(
                signInDto.Email,
                signInDto.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(signInDto);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index), "Blog");
        }




        [HttpPost]
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }
    } // end class
} // end namepace