using System;

namespace RedisClient
{
    [Serializable]
    public class ComplexCacheItem
    {
        public Guid SessionKey { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public ChildObject ChildObject { get; set; }

        public ComplexCacheItem()
        {
            SessionKey = Guid.NewGuid();
            Username = "user.name";
            Password = "redis";
            ChildObject = new ChildObject();
        }
    }


    [Serializable]
    public class ChildObject
    {
        public string Text { get; set; }

        public int CacheVisitCount { get; set; }

        public ChildObject()
        {
            CacheVisitCount = 1;
            Text = "This is simple redis client";
        }
    }
}