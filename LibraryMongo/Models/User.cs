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
        public string Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Category { get; set; }
        public DateTime MDate { get; set; }
    }
}