﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kahla.Home.Services;
using Kahla.Home.Models.HomeViewModels;
using Microsoft.Extensions.Caching.Memory;

namespace Kahla.Home.Controllers
{
    public class HomeController : Controller
    {
        private readonly VersionChecker _version;
        private readonly IMemoryCache _cache;
        public HomeController(
            VersionChecker version,
            IMemoryCache cache)
        {
            _version = version;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            if (!_cache.TryGetValue("Version.Cache", out string version))
            {
                version = await _version.CheckKahla();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(20));

                _cache.Set("Version.Cache", version, cacheEntryOptions);
            }
            var model = new IndexViewModel
            {
                LatestVersion = version
            };
            return View(model);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
