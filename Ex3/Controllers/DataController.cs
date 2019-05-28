using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ex3.Models.CommandsServer;

namespace Ex3.Controllers
{

    public class DataController : Controller
    {
        [HttpGet]
        public ActionResult Option1(string ip, int port)

        {
            CommandsServer commandsServer = CommandsServer.getInstance();
            commandsServer.Ip = ip;
            commandsServer.Port = port;
            commandsServer.connect();
            double lon = commandsServer.write("get position/longitude-deg");
            double lat = commandsServer.write("get position/latitude-deg");
            ViewBag.Lon = lon;
            ViewBag.Lat = lat;
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