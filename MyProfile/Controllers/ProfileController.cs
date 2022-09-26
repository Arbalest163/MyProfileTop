using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyProfile.Controllers {
    public class ProfileController : Controller 
    {

        private readonly MyProfileContext _dbContext;
        public ProfileController(MyProfileContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (Startup.userId == 0)
            {
                return Redirect("/");
            }

            var linkInfos = from info in _dbContext.UsersInfo
                         join nametype in _dbContext.TypesInfo on info.TypeInfo_id equals nametype.Id
                         where Startup.userId == info.User_id
                         select new LinkInfo
                         {
                             Id = info.Id,
                             NameType = nametype.NameType,
                             Info = info.Info
                         };

            var indexModel = new IndexModel
            {
                User = _dbContext.Users.FirstOrDefault(u => Startup.userId == u.Id),
                linkInfos = linkInfos.ToArray(),
                Messages = _dbContext.Records.Where(u => u.UserId == Startup.userId).OrderByDescending(r => r.Id).ToArray(),
                Pictures = _dbContext.PicturesOnTheWall.Where(x => x.UserId == Startup.userId).ToArray()
            };

            return View(indexModel);
        }

        [HttpGet]
        public IActionResult New() {
            return View("Reg");
        }

        [HttpPost]
        public IActionResult Create([FromForm]UserDto user) {

            User existsUser = _dbContext.Users.FirstOrDefault(u => user.username == u.Username);
            if (existsUser != null) {
                ViewData["Error"] = "Такой логин уже занят!";
                return View("Reg");
            }
            
            string photoUrl = "images/profiles/";
            if (user.photo != null) {
                int ts = (Int32)user.birthday.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                photoUrl += user.username + "_" + ts.ToString() + "." + (user.photo.FileName.Split('.'))[1];
                using var fileStream = new FileStream("wwwroot/" + photoUrl, FileMode.Create);
                user.photo.CopyTo(fileStream);
            }
            
            User newUser = new User {
                Username = user.username,
                Password = user.password,
                Email = user.email,
                Status = "Новый пользователь",
                Birthday = (Int32)user.birthday.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                PhotoProfile = photoUrl
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            return Redirect("/");
        }

        [HttpGet]
        public IActionResult AddContact() {
            List<TypeInfo> Types = _dbContext.TypesInfo.ToList();
            ViewBag.Types = Types;
            return View("AddContact");
        }
        [HttpPost]
        public IActionResult AddContact(int typecontact, string contactinfo) {
            _dbContext.UsersInfo.Add(new UserContactInfo {
                User_id = Startup.userId,
                TypeInfo_id = typecontact,
                Info = contactinfo,
            });
            _dbContext.SaveChanges();
            return Redirect("/profile");
        }
        [HttpGet]
        public IActionResult Login() {
            return View("Login");
        }
        [HttpPost]
        public IActionResult Auth(string login, string pass) {
            User user = _dbContext.Users.FirstOrDefault(n => n.Username == login);
            if (user != null) {
                if (user.Username == login && user.Password == pass) {
                    //авторизация верная
                    HttpContext.Response.Cookies.Append("Username", user.Username);
                    HttpContext.Response.Cookies.Append("UserId", user.Id.ToString());
                    return Redirect("/profile");
                } else {
                    ViewData["Error"] = "Ошибка авторизации";
                    return View("Login");
                }
            } else {
                ViewData["Error"] = "Ошибка авторизации";
                return View("Login");
            }
        }

        [HttpGet]
        public IActionResult Logout() {
            HttpContext.Response.Cookies.Delete("Username");
            HttpContext.Response.Cookies.Delete("UserId");
            Startup.isAuth = false;
            Startup.userId = 0;
            return Redirect("/");
        }
        [HttpGet]
        public IActionResult AddRecord() {
            return View("AddRecord");
        }
        [HttpPost]
        public IActionResult AddRecord(string title, string fulltext) {
            if (fulltext.Length > 1024) {
                fulltext = fulltext.Substring(0, 1024);
            }

            _dbContext.Records.Add(new Record {
                UserId = Startup.userId,
                Title = title,
                Message = fulltext,
            });
            _dbContext.SaveChanges();
            return Redirect("/profile");
        }

        [HttpGet]
        public IActionResult Delete(string type, int infoid) {
            if (type == "info") {
                DeleteInfo(infoid);
            } else if (type == "message") {
                DeleteMessage(infoid);
            }
            return Redirect("/profile");
        }

        private bool DeleteInfo(int id) {
            UserContactInfo ucinfo = _dbContext.UsersInfo.
                FirstOrDefault(u => u.Id == id && Startup.userId == u.User_id);
            if (ucinfo != null) { 
                _dbContext.UsersInfo.Remove(ucinfo);
                _dbContext.SaveChanges();
                return true;
            } else {
                return false;
            }
        }

        private bool DeleteMessage(int id) {
            Record rec = _dbContext.Records.
                FirstOrDefault(u => u.Id == id && Startup.userId == u.UserId);
            if (rec != null) {
                _dbContext.Records.Remove(rec);
                _dbContext.SaveChanges();
                return true;
            } else {
                return false;
            }
        }

        [HttpGet]
        public IActionResult Edit() {
            User user = _dbContext.Users.FirstOrDefault(u => u.Id == Startup.userId);
            if (user != null) {
                ViewBag.Usermail = user.Email;
                ViewBag.UserLogin = user.Username;
                ViewBag.UserStatus = user.Status;
                return View("Edit");
            }
            return Redirect("/profile");
        }

        [HttpGet]
        public IActionResult AddPicture()
        {
            return View("AddPicture");
        }
        [HttpPost]
        public IActionResult AddPicture([FromForm]PictureDto pictureDto)
        {
            string picturePath = "images/profiles/";
            if (pictureDto != null)
            {
                var dop = ((int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
                picturePath += Startup.userId + "_" + dop + "." + pictureDto.File.FileName.Split('.')[1];
                using var fileStream = new FileStream("wwwroot/" + picturePath, FileMode.Create);
                pictureDto.File.CopyTo(fileStream);
                var pictureWall = new PictureOnTheWall
                {
                    UserId = Startup.userId,
                    PicturePath = picturePath
                };
                _dbContext.PicturesOnTheWall.Add(pictureWall);
                _dbContext.SaveChanges();
            }

            return Redirect("/");
        }
    }

    public class LinkInfo {
        public int Id { get; set; }
        public string NameType { get; set; }
        public string Info { get; set; }
    }
}
