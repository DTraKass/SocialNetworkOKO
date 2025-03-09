using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SocialNetworkOKO.Models;
using SocialNetworkOKO.Repositories;

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

                var result = await _signInManager.PasswordSignInAsync(userData!, model.Password, model.RememberMe, false);

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

        [Authorize]
        [Route("MyPage")]
        [HttpGet]
        public async Task<IActionResult> MyPage()
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            var model = new UserViewModel(result);

            model.Friends = await GetAllFriend(model.User);

            return View("User", model);
        }

        private async Task<List<User>> GetAllFriend(User user)
        {
            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository.GetFriendsByUser(user);
        }

        [Route("Update")]
        [HttpGet]
        public async Task<IActionResult> Update(UserEditViewModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity!.Name!);

            return View("Edit", _mapper.Map<UserEditViewModel>(user));
        }

        private async Task<SearchViewModel> CreateSearch(string search)
        {
            var currentuser = User;
            var result = await _userManager.GetUserAsync(currentuser);

            var list = _userManager.Users.AsEnumerable();
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(x => x.GetFullName().ToLower().Contains(search.ToLower()));
            }

            var withfriend = await GetAllFriend();

            var data = new List<UserWithFriendExt>();
            foreach (var user in list)
            {
                var t = _mapper.Map<UserWithFriendExt>(user);
                t.IsFriendWithCurrent = withfriend.Any(y => y.Id == user.Id || user.Id == result!.Id);
                data.Add(t);
            }

            var model = new SearchViewModel()
            {
                UserList = data
            };

            return model;
        }

        private async Task<List<User>> GetAllFriend()
        {
            var user = User;

            var result = await _userManager.GetUserAsync(user);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            return repository!.GetFriendsByUser(result!);
        }

        [Route("UserList")]
        [HttpPost]
        public async Task<IActionResult> UserList(string search)
        {
            var model = await CreateSearch(search);
            return View("UserList", model);
        }

        [Route("AddFriend")]
        [HttpPost]
        public async Task<IActionResult> AddFriend(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);

            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            repository.AddFriend(result, friend);

            return RedirectToAction("MyPage", "AccountManager");

        }

        [Route("DeleteFriend")]
        [HttpPost]
        public async Task<IActionResult> DeleteFriend(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);

            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Friend>() as FriendsRepository;

            repository.DeleteFriend(result, friend);

            return RedirectToAction("MyPage", "AccountManager");

        }

        [Route("Chat")]
        [HttpPost]
        public async Task<IActionResult> Chat(string id)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var mess = repository.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };
            return View("Chat", model);
        }

        [Route("NewMessage")]
        [HttpPost]
        public async Task<IActionResult> NewMessage(string id, ChatViewModel chat)
        {
            var currentuser = User;

            var result = await _userManager.GetUserAsync(currentuser);
            var friend = await _userManager.FindByIdAsync(id);

            var repository = _unitOfWork.GetRepository<Message>() as MessageRepository;

            var item = new Message()
            {
                Sender = result,
                Recipient = friend,
                Text = chat.NewMessage.Text,
            };
            chat.NewMessage.Timestamp = DateTime.Now;
            repository.Create(item);

            var mess = repository.GetMessages(result, friend);

            var model = new ChatViewModel()
            {
                You = result,
                ToWhom = friend,
                History = mess.OrderBy(x => x.Id).ToList(),
            };
            return View("Chat", model);
        }

        public class GenerateUsers
        {
            private Random _random = new Random();

            public List<User> Populate(int count)
            {
                var userList = new List<User>();

                for (int i = 0; i < count; i++)
                {
                    userList.Add(new User
                    {
                        FirstName = GenerateRandomString(5),
                        LastName = GenerateRandomString(7),
                        MiddleName = GenerateRandomString(6),
                        BirthDate = GenerateRandomDate(),
                        Image = "default.png",
                        Status = "Online",
                        About = GenerateRandomString(20)
                    });
                }

                return userList;
            }

            private string GenerateRandomString(int length)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[_random.Next(s.Length)]).ToArray());
            }

            private DateTime GenerateRandomDate()
            {
                var start = new DateTime(1970, 1, 1);
                var end = new DateTime(2000, 1, 1);
                int range = (end - start).Days;
                return start.AddDays(_random.Next(range));
            }
        }

        [Route("Generate")]
        [HttpGet]
        public async Task<IActionResult> Generate()
        {
            var usergen = new GenerateUsers();
            var userlist = usergen.Populate(10);

            foreach (var user in userlist)
            {
                var identityUser = new User
                {
                    UserName = user.FirstName.ToLower() + user.LastName.ToLower(),
                    Email = user.FirstName.ToLower() + "@test",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MiddleName = user.MiddleName,
                    BirthDate = user.BirthDate,
                    Image = user.Image,
                    Status = user.Status,
                    About = user.About,
                };

                var result = await _userManager.CreateAsync(identityUser, "123456");

                if (!result.Succeeded)
                    continue;
            }

            return RedirectToAction("Index", "Home");
        }

        private IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;

        public AccountManagerController(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
    }
}
