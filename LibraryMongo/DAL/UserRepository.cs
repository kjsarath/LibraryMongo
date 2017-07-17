using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using LibraryMongo.Properties;
using LibraryMongo.Models;
using MongoDB.Driver.Builders;

namespace LibraryMongo.DAL
{
    public class UserRepository
    {
        public IMongoDatabase mDb;
        public MongoDB.Driver.IMongoCollection<User> users;
        public bool serverDown=false;
        public UserRepository()
        {
            //var credential = MongoCredential.CreateMongoCRCredential("admin", "mongoAdmin", "$cR@m$H@1");
            //var settings = new MongoClientSettings
            //{
            //    Credentials = new[] { credential },
            //    Server = new MongoServerAddress("DEMO",4509)
            //};
            //var mClient = new MongoClient(settings);
            var mClient = new MongoClient(Settings.Default.LibraryConnectionString);
            mDb = mClient.GetDatabase("Library");
            users = mDb.GetCollection<User>("Users");
            try
            {
                //var mServer=mClient.GetServer();
                //mServer.Ping();
            }
            catch(System.Exception ex)
            {
                serverDown = true;
            }
        }
        private User[] _testUserData = new User[]
        {
            new User()
            {
                userid = Guid.NewGuid() .ToString(),
                username = "User1",
                displayname = "User1",
                password = "User1",
                role = "admin",
                active=true 
            }
        };
        private List<User> _usersList = new List<User>();
        public IEnumerable<User> GetAllUsers()
        {
            if (serverDown ) return null;

            if (Convert.ToInt32(users.Count(FilterDefinition<User>.Empty )) > 0)
            {
                _usersList.Clear();
                var us = users.Find(FilterDefinition<User>.Empty).ToList();
                if (us.Count > 0)
                {
                    foreach (User u in us)
                    {
                        _usersList.Add(u);
                    }
                }
            }
            else
            {
                users.DeleteMany(FilterDefinition<User>.Empty);
                foreach (var u in _testUserData )
                {
                    _usersList .Add(u);

                    Add(u); 
                }
            }

            var result = _usersList.AsQueryable();
            return result;
        }
        public User  Add(User user)
        {
            if (string.IsNullOrEmpty(user.userid ))
            {
                user.userid = Guid.NewGuid().ToString();
            }
            users.InsertOne(user);
            return user;
        }
        public User GetUserById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("Id", "User Id is empty!");
            }
            //var employee = (User)users.FindOneAs(typeof(User), Query.EQ("_id", id));
            var builder = Builders<User>.Filter;
            var filter = builder.Eq("_id", id) ;
            var user = (User)users.Find(filter);
            return user;
        }
        public bool isValidUser(string uname, string pword)
        {
            var c = users.Find(Builders<User>.Filter.Eq("username", uname) & Builders<User>.Filter.Eq("password", pword)).Count();
            if (c == 1) return true;
            else return false;
        }
        public bool Update(string objectId, User user)
        {
            var updateBuilder = Builders<User>.Update
                .Set("username", user.username)
                .Set("displayname", user.displayname)
                .Set("password", user.password)
                .Set("role", user.role)
                .Set("active", user.active);
            var result = users.UpdateOne (Builders<User>.Filter.Eq("_id", objectId), updateBuilder);
            return true;
        }
        public bool Delete(string objectId)
        {
            users.DeleteOne(Builders<User>.Filter.Eq("_id", objectId));
            return true;
        }
    }
}