using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CachingRedis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;

        public CacheController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        [HttpGet("{key}")]
        public IActionResult GetCache(string key)
        {
            string value = string.Empty;
            memoryCache.TryGetValue(key, out value);
            return Ok(value);
        }


        //{
        //    "key":"Name",
        //    "value": "Emon"
        //}

        [HttpPost]
        public IActionResult SetCache(CacheRequest data)
        {
            var cacheExpiryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                //Absolute Expiration – The problem with Sliding Expiration is that theoretically, it can last forever. Let’s say someone requests for the data every 1.59 minutes for the next couple of days, the application would be technically serving an outdated cache for days together. With Absolute expiration, we can set the actual expiration of the cache entry. Here it is set as 5 minutes.So, every 5 minutes, without taking into consideration the sliding expiration, the cache will be expired.It’s always a good practice to use both these expirations checks to improve performance. But remember, the absolute expiration SHOULD NEVER BE LESS than the Sliding Expiration.

                Priority = CacheItemPriority.High,
                //Priority – Sets the priority of keeping the cache entry in the cache.The default setting is Normal.Other options are High,Low and Never Remove.This is pretty self - explanatory.
                SlidingExpiration = TimeSpan.FromMinutes(2),
                //Sliding Expiration – A defined Timespan within which a cache entry will expire if it is not used by anyone for this particular time period.In our case, we set it to 2 minutes.If, after setting the cache, no client requests for this cache entry for 2 minutes, the cache will be deleted.

                Size = 1024,
                //Size – Allows you to set the size of this particular cache entry, so that it doesn’t start consuming the server resources.

            };
            memoryCache.Set(data.key, data.value, cacheExpiryOptions);
            return Ok();
        }



    public class CacheRequest
        {
            public string key { get; set; }
            public string value { get; set; }
        }
    }
}
