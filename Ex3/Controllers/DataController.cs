using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ex3.Controllers
{
    public class DataController : Controller
    {
        [HttpGet]
        public ActionResult Option1(string ip, int port)
        {
            //
            return View();
        }

        [HttpGet]
        public ActionResult Option2(string ip, int port, int samples)
        {
            //
            return View();
        }
    }
}