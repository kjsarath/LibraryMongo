using LibraryMongo.DAL;
using LibraryMongo.Models;
using LibraryMVC.DAL;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
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
            object bC = null;
            try
            {
                bC = (books.books.Aggregate().Project(Builders<Books>.Projection.Include("Code").Exclude("_id")).Sort(Builders<BsonDocument>.Sort.Descending("Code")).Limit(1).ToList()[0].ElementAt(0).Value);
                bC = int.Parse(bC.ToString()) + 1;
            }
            catch(Exception)
            {
                bC = null;
            }
            if (bC != null) b.Code = bC.ToString();
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
                Books b = books.getAllBooks().Where(bk => bk.BookID == BookID).FirstOrDefault();
                if(TryUpdateModel(b))
                {
                    try
                    {
                        books.update(b);
                        return RedirectToAction("Index");
                    }
                    catch(RetryLimitExceededException )
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
                    }
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MultipleButton(Name ="action",Argument ="Back")]
        public ActionResult Back()
        {
            return RedirectToAction("Index");
        }

        public ActionResult Details(string id)
        {
            var bk = books.getAllBooks().Where(b => b.BookID == id.ToString()).FirstOrDefault();
            return View(bk);
        }

        [HttpPost]
        [MultipleButton(Name ="action",Argument ="Edit")]
        public ActionResult Edit(string id)
        {
            var bk = books.getAllBooks().Where(b => b.BookID == id).FirstOrDefault();
            return View("Add", bk);
        }

        [HttpPost]
        [MultipleButton(Name ="action",Argument ="AddCopy")]
        public ActionResult AddCopy(string id)
        {
            Copies cp = new Copies();
            object  allDocs=null;
            try
            {
                //allDocs = ((books.books as IMongoCollection<Books>).Find(FilterDefinition<Books>.Empty).Project(Builders<Books>.Projection.Include("Copies.AccessionNo").Exclude("_id")).Sort(Builders<Books>.Sort.Descending("Copies.AccessionNo")).Limit(1).ToList()[0].ElementAt(0).Value)[0].AsBsonDocument.ElementAt(0).Value;
                allDocs = ((books.books as IMongoCollection<Books>).Aggregate().Project(Builders<Books>.Projection.Include("Copies.AccessionNo").Exclude("_id")).Unwind("Copies").Sort(Builders<BsonDocument>.Sort.Descending("Copies.AccessionNo")).Limit(1).ToList()[0].ElementAt(0).Value)[0].AsString     ;
                allDocs = int.Parse( allDocs.ToString()) + 1;
            }
            catch (Exception ex)
            {
                allDocs = null;
            }
            if (allDocs!=null )cp.AccessionNo = allDocs.ToString();
            return View("AddCopy",cp);
        }

        [HttpPost]
        [MultipleButton(Name ="action",Argument ="SaveCopy")]
        public ActionResult SaveCopy(string id)
        {
            Copies cp = new Models.Copies();
            if(TryUpdateModel(cp))
            {
                try
                {
                    books.addCopy(id, cp);
                    return RedirectToAction("Details", new { id = id });
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
                }
            }
            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult EditCopy(string id, string bookid)
        {
            Copies cp = books.getCopy(id);
            ViewBag.BookID = bookid;
            return View(cp);
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "SaveAsCopy")]
        public ActionResult SaveAsCopy(string id,string bookid)
        {
            Copies cp = books.getCopy(id);
            if (TryUpdateModel(cp))
            {
                try
                {
                    books.UpdateCopy(bookid, cp);
                    return RedirectToAction("Details", new { id = bookid });
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
                }
            }
            return RedirectToAction("Details", new { id = bookid });
        }
    }
}