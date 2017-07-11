using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LibraryMongo.Properties;
using LibraryMongo.Models;
using MongoDB.Driver;
using LibraryMongo.DAL;

namespace LibraryMongo.Controllers
{
    public class HomeController : Controller
    {
        public IMongoDatabase mDb;
        public UserRepository Context = new UserRepository();
        public HomeController()
        {
            var mClient = new MongoClient(Settings.Default.LibraryConnectionString);
            mDb  = mClient.GetDatabase("Library");
        }

        public ActionResult Index()
        {
            return View("Index", Context.GetAllUsers().ToList());
        }
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(User  user)
        {
            var result = Context.Add(user);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(string Id)
        {
            var employee = Context.GetUserById(Id);

            return View(Context.GetUserById(Id));
        }

        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (user == null) return RedirectToAction("Index");

            Context.Update(user.userid, user);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(string Id)
        {
            Context.Delete(Id);
            return RedirectToAction("Index");
        }
    }
}