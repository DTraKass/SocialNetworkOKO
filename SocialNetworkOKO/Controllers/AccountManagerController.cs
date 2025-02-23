using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SocialNetworkOKO.Models;

namespace SocialNetworkOKO.Controllers
{
    public class AccountManagerController : Controller
    {
        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Home/Login");
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var userData = await _userManager.FindByEmailAsync(model.Email);

                var result = await _signInManager.PasswordSignInAsync(userData, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("MyPage", "AccountManager");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View("Views/Home/Index.cshtml");
        }

        [Route("Logout")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Route("MyPage")]
        [HttpGet]
        [Authorize]
        public IActionResult MyPage()
        {
            var user = User;

            var result = _userManager.GetUserAsync(user);

            return View("User", new UserViewModel(result.Result));
        }

        [Route("Update")]
        [HttpGet]
        public async Task<IActionResult> Update(UserEditViewModel model)
        {

            //if (ModelState.IsValid)
            //{
                //var user = await _userManager.FindByIdAsync(model.UserId);
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                //user.Convert(model);
                return View("Edit", _mapper.Map<UserEditViewModel>(user));

                //var result = await _userManager.UpdateAsync(user);
                //if (result.Succeeded)
                //{
                //    return RedirectToAction("MyPage", "AccountManager");
                //}
                //else
                //{
                //    return RedirectToAction("Edit", "AccountManager");
                //}
            //}
            //else
            //{
            //    ModelState.AddModelError("", "Некорректные данные");
            //    return View("Edit", model);
            //}
        }

        [Route("UserList")]
        [HttpPost]
        public IActionResult UserList(string search)
        {
            var model = new SearchViewModel
            {
                UserList = _userManager.Users.AsEnumerable().Where(x => x.GetFullName().Contains(search)).ToList()
            };
            return View("UserList", model);
        }

        private IMapper _mapper;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountManagerController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }
    }
}
