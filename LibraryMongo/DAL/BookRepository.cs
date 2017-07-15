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
        public List<Books> getAllBooks()
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
            var result = booksList;
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
        public Books update(Books book)
        {
            if (string.IsNullOrEmpty(book.BookID))
            {
                return add(book);
            }
            var updateBuilder = Builders<Books>.Update
                .Set("ArticleCategory", book.ArticleCategory )
                .Set("ArticleType", book.ArticleType )
                .Set("Author1", book.Author1 )
                .Set("Author2", book.Author2 )
                .Set("Barcode", book.Barcode )
                .Set("CallNoP1", book.CallNoP1 )
                .Set("CallNoP2", book.CallNoP2 )
                .Set("CallNoP3", book.CallNoP3 )
                .Set("Category", book.Category )
                .Set("Code", book.Code )
                .Set("Copies", book.Copies )
                .Set("DateOfEntry",(book.DateOfEntry==null? DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) : DateTime.SpecifyKind((DateTime)book.DateOfEntry, DateTimeKind.Utc)  ))
                .Set("Description", book.Description )
                .Set("ISBN", book.ISBN )
                .Set("Language", book.Language )
                .Set("Publisher", book.Publisher )
                .Set("Series", book.Series )
                .Set("SubTitle", book.SubTitle )
                .Set("Title", book.Title )
                .Set("Type", book.Type )
                .Set("Volume", book.Volume );
            books.UpdateOne(Builders<Books>.Filter.Eq("BookID", book.BookID),updateBuilder);
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