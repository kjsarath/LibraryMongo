using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryMongo.Models
{
    public class User
    {
        [BsonId]
        public MongoDB.Bson.ObjectId  _id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string displayname { get; set; }
        public Boolean  active { get; set; }
        public string role { get; set; }
        public string userid { get; set; }
    }
}