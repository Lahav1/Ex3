using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Ex3.Models.CommandsServer;

namespace Ex3.Controllers
{
    public class MapController : Controller
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
            ViewBag.UpdateRate = arg3;
            commandsServer.connect();
            return View();
        }

        [HttpPost]
        public string GetValues()
        {
            CommandsServer commandsServer = CommandsServer.getInstance();
            commandsServer.connect();
            double lon = commandsServer.write("get position/longitude-deg");
            double lat = commandsServer.write("get position/latitude-deg");
            return ToXML(lon.ToString() + " " + lat.ToString());
        }

        public string ToXML(string coordinates)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("coordinates");
            string[] temp = coordinates.Split(' ');
            writer.WriteElementString("Lon", temp[0]);
            writer.WriteElementString("Lat", temp[1]);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }
    }
}
