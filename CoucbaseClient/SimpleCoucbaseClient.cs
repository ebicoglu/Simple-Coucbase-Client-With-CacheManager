using System;
using System.Collections.Generic;
using CacheManager.Core;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using RedisClient;

namespace CoucbaseClient
{
    public class SimpleCoucbaseClient
    {
        private readonly ICacheManager<ComplexCacheItem> _cache;
        private readonly ClientConfiguration _clientConfiguration;

        public SimpleCoucbaseClient(string hostUrl, string bucketName, string bucketUsername, string bucketPassword)
        {
            _clientConfiguration = new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(hostUrl) },
                BucketConfigs = new Dictionary<string, BucketConfiguration> {
                  {bucketName, new BucketConfiguration {
                      PoolConfiguration = new PoolConfiguration {
                          MaxSize = 6,
                          MinSize = 4,
                          SendTimeout = 12000
                      },
                      DefaultOperationLifespan = 123,
                      BucketName = bucketName,
                      Username = bucketUsername,
                      Password = bucketPassword
                  }}
                }
            };


            _cache = CacheFactory.Build<ComplexCacheItem>(settings =>
            {
                settings
                    .WithCouchbaseConfiguration("couch", _clientConfiguration)
                    .WithCouchbaseCacheHandle("couch", bucketName)
                    .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(10))
                    .And
                    .WithMaxRetries(5)
                    .WithRetryTimeout(100);
            });



        }

        public void N1SqlQueryWithNativeCoucbaseObject(string query)
        {
            /*http://docs.couchbase.com/developer/dotnet-2.1/n1ql-queries.html*/

            using (var cluster = new Cluster(_clientConfiguration))
            {
                IBucket bucket = null;
                try
                {
                    bucket = cluster.OpenBucket();

                    var result = bucket.Query<dynamic>(query);
                    foreach (var row in result.Rows)
                    {
                        Console.WriteLine(row);
                        Console.WriteLine(new string('_', 30));
                    }
                }
                finally
                {
                    if (bucket != null)
                    {
                        cluster.CloseBucket(bucket);
                    }
                }
            }
        }

        public void SimpleJobs()
        {
            const string key = "sampleKey";

            var complexItemFromCache = _cache.Get<ComplexCacheItem>(key);
            if (complexItemFromCache != null)
            {
                Console.WriteLine("Item already exists in cache.");
            }


            //add new item or update if exists
            var complexItem = new ComplexCacheItem();
            _cache.AddOrUpdate(key, complexItem, item => complexItem);


            //get item from cache
            complexItemFromCache = _cache.Get<ComplexCacheItem>(key);

            //modify data
            complexItemFromCache.ChildObject.CacheVisitCount++;

            //update in cache server
            var cache = complexItemFromCache;
            _cache.Update(key, item => cache);

            //check data
            complexItemFromCache = _cache.Get<ComplexCacheItem>(key);
            if (complexItemFromCache.ChildObject.CacheVisitCount == 2)
            {
                Console.WriteLine("success!");
            }


        }
    }
}