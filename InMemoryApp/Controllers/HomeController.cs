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

            _memoryCache.Set<string>("time",DateTime.Now.ToString(),options);
        }

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
        ViewBag.time = timeCache;
        
        return View();
    }

  
}
