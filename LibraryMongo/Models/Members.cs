using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryMongo.Models
{
    public class Members
    {
        [BsonId]
        public MongoDB.Bson.ObjectId _id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string ContactNum { get; set; }
        public string ContactNumAlt { get; set; }
        public string ContactEmail { get; set; }
        public Nullable<System.DateTime> JoiningDate { get; set; }
        public string Nationality { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Area { get; set; }
        public string Emirates { get; set; }
        public string Status { get; set; }
        public byte[] Image { get; set; }
        public string Remarks { get; set; }
    }
}