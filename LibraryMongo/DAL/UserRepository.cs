using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using LibraryMongo.Properties;
using LibraryMongo.Models;

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
                var mServer=mClient.GetServer();
                mServer.Ping();
            }
            catch(System.Exception ex)
            {
                serverDown = true;
            }
        }

    }
}