using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LibraryMongo.Properties;
using LibraryMongo.Models;
using MongoDB.Driver;

namespace LibraryMongo.Controllers
{
    public class HomeController : Controller
    {
        public IMongoDatabase mDb;
        
        public HomeController()
        {
            var mClient = new MongoClient(Settings.Default.LibraryConnectionString);
            mDb  = mClient.GetDatabase("Library");
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
    }
}