using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UrlShortener.Data;
using UrlShortener.Models;
using shortid;

namespace UrlShortener.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var repo = new UserRepository(Properties.Settings.Default.ConStr);
            var user = repo.GetByEmail(User.Identity.Name);
            return View(user);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(string email, string password)
        {
            var repo = new UserRepository(Properties.Settings.Default.ConStr);
            var user = repo.LogIn(email, password);
            if (user == null)
            {
                return Redirect("/home/login");
            }

            FormsAuthentication.SetAuthCookie(user.EmailAddress, false);
            return Redirect("/home/index");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user, string password)
        {
            var repo = new UserRepository(Properties.Settings.Default.ConStr);
            repo.AddUser(user, password);
            return Redirect("/home/index");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return View();
        }

        [Authorize]
        public ActionResult Shorten()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Shorten(string longUrl)
        {
            var repo = new ActionsRepository(Properties.Settings.Default.ConStr);
            var result = repo.GetUrl(User.Identity.Name, longUrl);
            if (result == null)
            {
                var userRepo = new UserRepository(Properties.Settings.Default.ConStr);
                var user = userRepo.GetByEmail(User.Identity.Name);
                var shorty = ShortId.Generate(true,false);
                result = new URL
                {
                    UrlShort = shorty,
                    UrlOrig = longUrl,
                    UserId = user.Id
                };
                repo.AddUrl(result);
            }
            return Json(new { ShortUrl = FullShortUrl(result.UrlShort) });
        }

        [Route("{urlShort}")]
        public ActionResult ViewShortenedUrl(string urlShort)
        {
            var repo = new ActionsRepository(Properties.Settings.Default.ConStr);
            var url = repo.Get(urlShort);
            if (url == null)
            {
                return View("BadUrl");
            }
            repo.IncrementViews(url.Id);
            return Redirect(url.UrlOrig);
        }

        [Authorize]
        public ActionResult History()
        {
            var repo = new ActionsRepository(Properties.Settings.Default.ConStr);
            var userRepo = new UserRepository(Properties.Settings.Default.ConStr);
            var vm = new UserHistoryViewModel();
            vm.FirstName = userRepo.GetByEmail(User.Identity.Name).FirstName;
            vm.LastName = userRepo.GetByEmail(User.Identity.Name).LastName;
            vm.Urls = repo.GetHistory(User.Identity.Name);
            return View(vm);
        }

        private string FullShortUrl(string shorty)
        {
            var repo = new ActionsRepository(Properties.Settings.Default.ConStr);
            return Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, "") + $"/{shorty}";
        }
    }
}