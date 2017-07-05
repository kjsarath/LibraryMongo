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
                Id = Guid.NewGuid().ToString(),
                UserName = "User1",
                DisplayName = "User1",
                Password = "New Delhi",
                Category = "111111111"
            },
            new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "User2",
                DisplayName = "User2",
                Password = "Mumbai",
                Category = "222222222"
            },
            new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "User3",
                DisplayName = "User3",
                Password = "Chennai",
                Category = "333333333"
            },
            new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "User4",
                DisplayName = "User4",
                Password = "Gurgaon",
                Category = "444444444"
            },
            new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "User5",
                DisplayName = "User5",
                Password = "Patna",
                Category = "555555555"
            },
            new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "User6",
                DisplayName = "User6",
                Password = "Varanasi",
                Category = "666666666"
            },
            new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "User7",
                DisplayName = "User7",
                Password = "New Delhi",
                Category = "777777777"
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
            if (string.IsNullOrEmpty(user.Id))
            {
                user.Id = Guid.NewGuid().ToString();
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
            var filter = builder.Eq("Id", id) ;
            var user = (User)users.Find(filter);
            return user;
        }
        public bool Update(string objectId, User user)
        {
            var updateBuilder = Builders<User>.Update
                .Set("UserName", user.UserName)
                .Set("DisplayName", user.DisplayName)
                .Set("Password", user.Password)
                .Set("Category", user.Category);
            var result = users.UpdateOne (Builders<User>.Filter.Eq("Id", objectId), updateBuilder);
            return true;
        }
        public bool Delete(string objectId)
        {
            users.DeleteOne(Builders<User>.Filter.Eq("Id", objectId));
            return true;
        }
    }
}