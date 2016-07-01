using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LubCycle.Api.Controllers
{
    public class NaviController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
