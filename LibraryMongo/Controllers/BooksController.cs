using LibraryMongo.DAL;
using LibraryMongo.Models;
using LibraryMVC.DAL;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

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
            if (bL!=null && bL.Count > 0)
            {
                return View(bL);
            }
            else
            {
                return RedirectToAction("AddFirst");
            }
        }

        public ActionResult AddFirst()
        {
            Books b = new Books();
            object bC = null;
            try
            {
                bC = (books.books.Aggregate().Project(Builders<Books>.Projection.Include("Code").Exclude("_id")).Sort(Builders<BsonDocument>.Sort.Descending("Code")).Limit(1).ToList()[0].ElementAt(0).Value);
                bC = int.Parse(bC.ToString()) + 1;
            }
            catch (Exception)
            {
                bC = null;
            }
            if (bC != null) b.Code = bC.ToString();
            return View("Add");
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

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Search")]
        public ActionResult Search(string txtISBN)
        {
            Books bI = new Models.Books();
            getFromISBN(bI, txtISBN);
            object bC = null;
            try
            {
                bC = (books.books.Aggregate().Project(Builders<Books>.Projection.Include("Code").Exclude("_id")).Sort(Builders<BsonDocument>.Sort.Descending("Code")).Limit(1).ToList()[0].ElementAt(0).Value);
                bC = int.Parse(bC.ToString()) + 1;
            }
            catch (Exception)
            {
                bC = null;
            }
            if (bC != null) bI.Code = bC.ToString();
            return View("Add", bI);
        }

        [NonAction]
        private void getFromISBN(Books bookD,string isbn)
        {
            String ISBN = isbn;


            try
            {
                System.Net.HttpWebRequest oHTTPRequest = System.Net.HttpWebRequest.Create("https://www.googleapis.com/books/v1/volumes?q=isbn:" + ISBN) as System.Net.HttpWebRequest;
                oHTTPRequest.ContentType = "Json";
                string result = "";
                try
                {
                    Stream response = oHTTPRequest.GetResponse().GetResponseStream();
                    StreamReader reader = new StreamReader(response);
                    result = reader.ReadToEnd();
                }
                finally
                {
                }
                System.Xml.XmlDocument doc = JsonConvert.DeserializeXmlNode(result, "root");
                XmlReader xmlReader = new XmlNodeReader(doc);
                DataSet ds = new DataSet();
                ds.ReadXml(xmlReader);
                if (ds.Tables.Contains("industryIdentifiers"))
                {
                    DataColumnCollection columns = ds.Tables["industryIdentifiers"].Columns;
                    if (columns.Contains("identifier"))
                    {
                        if (ds.Tables["IndustryIdentifiers"].Rows[0]["type"].ToString() == "ISBN_13" || ds.Tables["IndustryIdentifiers"].Rows[0]["type"].ToString() == "OTHER")
                        {
                            bookD.ISBN = ds.Tables["industryIdentifiers"].Rows[0]["identifier"].ToString();
                        }
                        else
                        {
                            bookD.ISBN = ds.Tables["industryIdentifiers"].Rows[1]["identifier"].ToString();
                        }
                    }
                    else
                    {
                        bookD.ISBN = "";
                    }
                }
                if (ds.Tables.Contains("imageLinks"))
                {
                    DataColumnCollection columns = ds.Tables["imageLinks"].Columns;
                    if (columns.Contains("thumbnail"))
                    {
                        string sURL = ds.Tables["imageLinks"].Rows[0]["thumbnail"].ToString().Trim();
                        System.Net.WebClient webClient = new System.Net.WebClient();
                        //bookD.img = webClient.DownloadData(sURL);
                        //img = webClient.DownloadData(sURL);
                        //imgBook.Value  = sURL;
                    }
                    else
                    {
                    }
                }
                if (ds.Tables.Contains("volumeInfo"))
                {
                    DataColumnCollection columns = ds.Tables["volumeInfo"].Columns;
                    string title;
                    if (columns.Contains("Title"))
                    {
                        title = ds.Tables["volumeInfo"].Rows[0]["title"].ToString();
                        bookD.Title = title.Replace("'", "");
                    }
                    else
                    {
                        bookD.Title = "";
                    }
                    string subtitle;
                    if (columns.Contains("Subtitle"))
                    {
                        subtitle = ds.Tables["volumeInfo"].Rows[0]["Subtitle"].ToString();
                        bookD.SubTitle = subtitle.Replace("'", "");
                    }
                    else
                    {
                        bookD.SubTitle = "";
                    }
                    string catgory;
                    if (columns.Contains("categories"))
                    {
                        catgory = ds.Tables["volumeInfo"].Rows[0]["categories"].ToString();
                        bookD.Category = catgory.Replace("'", "");
                    }
                    else
                    {
                        bookD.Category = "";
                    }
                    string lang;
                    if (columns.Contains("Language"))
                    {
                        lang = ds.Tables["volumeInfo"].Rows[0]["Language"].ToString();
                        bookD.Language = lang.Replace("'", "");
                    }
                    else
                    {
                        bookD.Language = "English";
                    }
                    string desc;
                    if (columns.Contains("Description"))
                    {
                        desc = ds.Tables["volumeInfo"].Rows[0]["Description"].ToString();
                        bookD.Description = desc.Replace("'", "");
                    }
                    else
                    {
                        bookD.Description = "";
                    }
                    string pub;
                    if (columns.Contains("publisher"))
                    {
                        pub = ds.Tables["volumeInfo"].Rows[0]["publisher"].ToString();
                        bookD.Publisher = pub.Replace("'", "");
                    }
                    else
                    {
                        bookD.Publisher = "";
                    }
                    string aut1;
                    string aut2;
                    if (ds.Tables.Contains("authors"))
                    {
                        if (ds.Tables["authors"].Rows.Count > 0)
                        {
                            if (ds.Tables["authors"].Rows.Count <= 2)
                            {
                                aut1 = ds.Tables["authors"].Rows[0]["authors_Text"].ToString();
                                aut2 = ds.Tables["authors"].Rows[1]["authors_Text"].ToString();
                                bookD.Author1 = aut1.Replace("'", "");
                                bookD.Author2 = aut2.Replace("'", "");
                            }
                        }
                    }
                    else if (columns.Contains("authors"))
                    {
                        aut1 = ds.Tables["volumeInfo"].Rows[0]["authors"].ToString();
                        bookD.Author1 = aut1.Replace("'", "");
                    }
                    else
                    {
                        bookD.Author1 = "";
                    }

                }
                else
                {
                    System.Net.HttpWebRequest oHTTPRequest1 = System.Net.HttpWebRequest.Create("http://xisbn.worldcat.org/webservices/xid/isbn/ " + isbn + "?method=getMetadata&format=xml&fl=*&callback=mymethod") as System.Net.HttpWebRequest;
                    oHTTPRequest1.ContentType = "xml";
                    string result1 = "";
                    try
                    {
                        Stream response1 = oHTTPRequest1.GetResponse().GetResponseStream();
                        StreamReader reader1 = new StreamReader(response1);
                        result1 = reader1.ReadToEnd();


                    }
                    finally
                    {
                    }

                    System.Xml.XmlDocument doc1 = new System.Xml.XmlDocument();
                    doc1.LoadXml(result1);
                    XmlReader xmlReaderX = new XmlNodeReader(doc1);
                    DataSet ds1 = new DataSet();
                    ds.ReadXml(xmlReader);
                    if (ds1.Tables.Contains("isbn"))
                    {
                        DataColumnCollection columns = ds1.Tables["isbn"].Columns;
                        if (columns.Contains("isbn_Text"))
                        {
                            bookD.ISBN = ds1.Tables["isbn"].Rows[0]["isbn_Text"].ToString();
                            string pub2;
                            if (columns.Contains("publisher"))
                            {
                                pub2 = ds1.Tables["isbn"].Rows[0]["publisher"].ToString();
                                bookD.Publisher = pub2.Replace("'", "");
                            }
                            else
                            {
                                bookD.Publisher = "";
                            }
                            string auth1;
                            if (columns.Contains("author"))
                            {
                                auth1 = ds1.Tables["isbn"].Rows[0]["author"].ToString();
                                bookD.Author1 = auth1.Replace("'", "");
                            }
                            else
                            {
                                bookD.Author1 = "";
                            }

                            string title1;
                            if (columns.Contains("title"))
                            {
                                title1 = ds1.Tables["isbn"].Rows[0]["title"].ToString();
                                bookD.Title = title1.Replace("'", "");
                            }
                            else
                            {
                                bookD.Title = "";
                            }
                            if (columns.Contains("lang"))
                            {
                                bookD.Language = ds1.Tables["isbn"].Rows[0]["lang"].ToString();
                            }
                            else
                            {
                                bookD.Language = "";
                            }
                        }
                        else
                        {
                            bookD.ISBN = "";
                        }

                    }
                    else
                    {
                        //useISBNSearchAPI(isbn);
                    }
                }
                bookD.ISBN = ISBN;
            }
            catch (System.Exception)
            {

            }
        }

        [HttpPost]
        [MultipleButton(Name = "action", Argument = "BackToBook")]
        public ActionResult BackToBook(string bookid)
        {
            return RedirectToAction("Details",new { id = bookid });
        }
    }
}