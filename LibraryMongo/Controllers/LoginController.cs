using LibraryMongo.DAL;
using LibraryMongo.Models;
using LibraryMongo.Properties;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LibraryMongo.Controllers
{
    public class LoginController : Controller
    {
        public IMongoDatabase mDb;
        public UserRepository Context = new UserRepository();
        public LoginController()
        {
            var mClient = new MongoClient(Settings.Default.LibraryConnectionString);
            mDb = mClient.GetDatabase("Library");
        }

        [HttpGet ]
        public ActionResult Index()
        {
            Session["LoginInfo"] = null;
            return View();
        }

        [HttpPost ]
        public ActionResult Index(User uU)
        {
            if (Context.isValidUser(uU.username, uU.password) == true)
            {
                FormsAuthentication.SetAuthCookie(uU.username, true);
                Session["LoginInfo"] = uU.username;
                return RedirectToAction("Index", "Books");
            }
            else
            {
                ViewBag.Message = "Your credentials are wrong...";
                return View();
            }
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}