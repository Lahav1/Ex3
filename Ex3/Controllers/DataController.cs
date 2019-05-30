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
        public ActionResult LocationDisplay(string arg1, int arg2)
        {
            CommandsServer commandsServer = CommandsServer.getInstance();
            commandsServer.Ip = arg1;
            commandsServer.Port = arg2;
            commandsServer.connect();
            double lon = commandsServer.write("get position/longitude-deg");
            double lat = commandsServer.write("get position/latitude-deg");
            ViewBag.Lon = lon;
            ViewBag.Lat = lat;
            return View();
        }

        [HttpGet]
        public ActionResult RouteDisplay(string arg1, int arg2, int arg3)
        {
            CommandsServer commandsServer = CommandsServer.getInstance();
            commandsServer.Ip = arg1;
            commandsServer.Port = arg2;
            commandsServer.connect();
            double lon = commandsServer.write("get position/longitude-deg");
            double lat = commandsServer.write("get position/latitude-deg");
            return View();
        }

        [HttpGet]
        public string GetValues()
        {
            CommandsServer commandsServer = CommandsServer.getInstance();
            commandsServer.connect();
            double lon = commandsServer.write("get position/longitude-deg");
            double lat = commandsServer.write("get position/latitude-deg");
            return lon.ToString() + " " + lat.ToString();
        }
    }
}