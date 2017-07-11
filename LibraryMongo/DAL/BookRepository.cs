using LibraryMongo.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryMongo.DAL
{
    public class BookRepository
    {
        public IMongoDatabase mDb;
        public MongoDB.Driver.IMongoCollection<Books> books;
        public bool serverDown = false;
        public BookRepository()
        {
            var mClient = new MongoClient(LibraryMongo.Properties.Settings.Default.LibraryConnectionString);
            mDb = mClient.GetDatabase("Library");
            books = mDb.GetCollection<Books>("Books");

        }
        private List<Books> booksList = new List<Books>();
        public IEnumerable<Books> getAllBooks()
        {
            if (serverDown) return null;
            if (Convert.ToInt32(books.Count(FilterDefinition<Books>.Empty)) > 0)
            {
                booksList.Clear();
                var bk = books.Find(FilterDefinition<Books>.Empty).ToList();
                if (bk.Count > 0)
                {
                    foreach (Books b in bk)
                    {
                        booksList.Add(b);
                    }
                }
            }
            else
            {
                return null;
            }
            var result = booksList.AsQueryable();
            return result;
        }
        public Books add(Books book)
        {
            if (string.IsNullOrEmpty(book.BookID))
            {
                book.BookID = Guid.NewGuid().ToString();
            }
            books.InsertOne(book);
            return book;
        }
        public Copies addCopy(string bookID,Copies copy)
        {
            if (string.IsNullOrEmpty(copy.CopyID))
            {
                copy.CopyID = Guid.NewGuid().ToString();
            }
            var filter = Builders<Books>.Filter.Eq("BookID",bookID);
            var update = Builders<Books>.Update.Push("Books.$.Copies", copy);
            books.FindOneAndUpdateAsync(filter, update);
            return copy;
        }
    }
}