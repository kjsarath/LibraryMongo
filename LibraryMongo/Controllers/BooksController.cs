using LibraryMongo.DAL;
using LibraryMongo.Models;
using LibraryMVC.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryMongo.Controllers
{
    public class BooksController : Controller
    {
        public BookRepository books = new BookRepository();
        public ActionResult Index()
        {
            if(ModelState.IsValid )
            {
                if(Session["LoginInfo"]==null)
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            var bL = books.getAllBooks();
            return View(bL);
        }

        [HttpPost ]
        [MultipleButton(Name ="action",Argument = "Add")]
        public ActionResult Add(string BookID)
        {
            Books b = new Books();
            return View(b);
        }

        [HttpPost]
        [MultipleButton(Name ="action",Argument ="Save")]
        public ActionResult Save(string BookID)
        {
            if(string.IsNullOrEmpty(BookID ))
            {
                Books b = new Books();
                if(TryUpdateModel(b))
                {
                    try
                    {
                        books.add(b);
                        return RedirectToAction("Index");
                    }
                    catch (RetryLimitExceededException)
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MultipleButton(Name ="action",Argument ="Back")]
        public ActionResult Back()
        {
            return RedirectToAction("Index");
        }
    }
}