using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InMemoryApp.Models;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryApp.Controllers;

public class HomeController : Controller
{
    private readonly IMemoryCache _memoryCache;
    public HomeController(IMemoryCache memoryCache)
    {
      _memoryCache = memoryCache;
    }

    public IActionResult Index()
    {
        //set a memory cache key and value 
        if(!_memoryCache.TryGetValue("time", out string timeCache))
        {
            var options =new MemoryCacheEntryOptions();

            //time limit 10 seconds for cache
            options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);
            
            //expritian time is increse 5 sec per refresh
            options.SlidingExpiration = TimeSpan.FromSeconds(5);

            //Priorty is using for one of the cache is removed if somthing happen in memory. For example if memory limit is reached which caches should be remove
            options.Priority = CacheItemPriority.Normal;

            //Give an information about why a cache removed from memory
            options.RegisterPostEvictionCallback((key,value,reason,state)=>{
                _memoryCache.Set("callback",$"key: {key}, value: {value}, reason={reason}");
            });

            _memoryCache.Set<string>("time",DateTime.Now.ToString(),options);
        }

        //Complex type cache sample
        var user = new User{Id= Guid.NewGuid(), Name="John", SureName="Wick"};

        _memoryCache.Set<User>("userCache", user);

        return View();
    }

    public IActionResult Show()
    {
        //way 1 get a memory cache
        //ViewBag.time= _memoryCache.Get<string>("time");    

        //way 2 get or create a memory cache by key
        //ViewBag.time=_memoryCache.GetOrCreate<string>("time", entry=>{
        //return DateTime.Now.ToString();
        //});

        _memoryCache.TryGetValue("time", out string timeCache);
        _memoryCache.TryGetValue("callback", out string callback);
       

        ViewBag.time = timeCache;
        ViewBag.callback = callback;
        ViewBag.userCache = _memoryCache.Get<User>("userCache");
        
        return View();
    }

  
}
