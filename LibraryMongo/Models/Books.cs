using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryMongo.Models
{
    public class Books
    {
        [BsonId]
        public MongoDB.Bson.ObjectId _id { get; set; }
        public string BookID { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Author1 { get; set; }
        public string Author2 { get; set; }
        public string Type { get; set; }
        public string Series { get; set; }
        public string Publisher { get; set; }
        public string ISBN { get; set; }
        public string Barcode { get; set; }
        public string Volume { get; set; }
        public string Category { get; set; }
        public string ArticleType { get; set; }
        public string ArticleCategory { get; set; }
        public string Language { get; set; }
        public string Description { get; set; }
        public DateTime DateOfEntry { get; set; }
        public string CallNoP1 { get; set; }
        public string CallNoP2 { get; set; }
        public string CallNoP3 { get; set; }
        public List<Copies> Copies { get; set; }
    }
    public class Copies
    {

        public string CopyID { get; set; }
        public string SubCode { get; set; }
        public string AccessionNo { get; set; }
        public string Edition { get; set; }
        public string Pages { get; set; }
        public string PublishedYear { get; set; }
        public string BasicValue { get; set; }
        public string Currency { get; set; }
        public string PurchaseDate { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public string InOut { get; set; }
        public string CostInDhs { get; set; }
        public string Remarks { get; set; }
        public string IssueStatus { get; set; }
        public string Barcode { get; set; }
        public string PlaceOfPublication { get; set; }
        public string BookLanguage { get; set; }
    }
}