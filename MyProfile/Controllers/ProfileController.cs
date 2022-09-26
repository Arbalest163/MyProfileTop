using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace MyProfile.Controllers {
    public class ProfileController : Controller {

        MyProfileContext db;
        public ProfileController(MyProfileContext context) {
            db = context;
        }

        [HttpGet]
        public IActionResult Index() {
            if (Startup.userId == 0) {
                return Redirect("/");
            }

            User user = db.Users.FirstOrDefault(u => Startup.userId == u.Id);
            ViewBag.User = user;

            var result = from info in db.UsersInfo
                         join nametype in db.TypesInfo on info.TypeInfo_id equals nametype.Id
                         where Startup.userId == info.User_id
                         select new {
                             Id = info.Id,
                             NameType = nametype.NameType,
                             Info = info.Info
                         };
            List<LinkInfo> infos = new List<LinkInfo>();
            foreach(var info in result) {
                infos.Add(new LinkInfo{
                    Id = info.Id,
                    NameType = info.NameType,
                    Info = info.Info
                });
            }
            ViewBag.Infos = infos;

            List<Record> records = 
                db.Records.Where(u => u.User_id == Startup.userId).
                OrderByDescending(r => r.Id).ToList();
            ViewBag.Messages = records;
            return View();
        }

        [HttpGet]
        public IActionResult New() {
            return View("Reg");
        }

        [HttpPost]
        public IActionResult Create([FromForm]UserDto user) {

            User existsUser = db.Users.FirstOrDefault(u => user.username == u.Username);
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

            db.Users.Add(newUser);
            db.SaveChanges();

            return Redirect("/");
        }

        [HttpGet]
        public IActionResult AddContact() {
            List<TypeInfo> Types = db.TypesInfo.ToList();
            ViewBag.Types = Types;
            return View("AddContact");
        }
        [HttpPost]
        public IActionResult AddContact(int typecontact, string contactinfo) {
            db.UsersInfo.Add(new UserContactInfo {
                User_id = Startup.userId,
                TypeInfo_id = typecontact,
                Info = contactinfo,
            });
            db.SaveChanges();
            return Redirect("/profile");
        }
        [HttpGet]
        public IActionResult Login() {
            return View("Login");
        }
        [HttpPost]
        public IActionResult Auth(string login, string pass) {
            User user = db.Users.FirstOrDefault(n => n.Username == login);
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

            db.Records.Add(new Record {
                User_id = Startup.userId,
                Title = title,
                Message = fulltext,
            });
            db.SaveChanges();
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
            UserContactInfo ucinfo = db.UsersInfo.
                FirstOrDefault(u => u.Id == id && Startup.userId == u.User_id);
            if (ucinfo != null) { 
                db.UsersInfo.Remove(ucinfo);
                db.SaveChanges();
                return true;
            } else {
                return false;
            }
        }

        private bool DeleteMessage(int id) {
            Record rec = db.Records.
                FirstOrDefault(u => u.Id == id && Startup.userId == u.User_id);
            if (rec != null) {
                db.Records.Remove(rec);
                db.SaveChanges();
                return true;
            } else {
                return false;
            }
        }

        [HttpGet]
        public IActionResult Edit() {
            User user = db.Users.FirstOrDefault(u => u.Id == Startup.userId);
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
            var pictures = db.PicturesOnTheWall.Where(x => x.UserId == Startup.userId).ToArray();
            ViewBag.Picture = pictures;
            return View("AddPicture");
        }
        [HttpPost]
        public IActionResult AddPicture([FromForm]PictureDto pictureDto)
        {
            string picturePath = "images/profiles/";
            if (pictureDto != null)
            {
                picturePath += Startup.userId + "_" + DateTime.Now.ToString() + "." + pictureDto.File.FileName.Split('.')[1];
                using var fileStream = new FileStream("wwwroot/" + picturePath, FileMode.Create);
                pictureDto.File.CopyTo(fileStream);
                var pictureWall = new PictureOnTheWall
                {
                    UserId = Startup.userId,
                    PicturePath = picturePath
                };
                db.PicturesOnTheWall.Add(pictureWall);
                db.SaveChanges();
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
