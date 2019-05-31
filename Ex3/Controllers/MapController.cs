using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Ex3.Models.CommandsServer;

namespace Ex3.Controllers
{
    public class MapController : Controller
    {
        private static string filePath;

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
            return View("~/Views/Map/LocationDisplay.cshtml");
        }

        [HttpGet]
        public ActionResult RouteDisplay(string arg1, int arg2, int arg3)
        {
            CommandsServer commandsServer = CommandsServer.getInstance();
            commandsServer.Ip = arg1;
            commandsServer.Port = arg2;
            ViewBag.UpdateRate = arg3;
            commandsServer.connect();
            double lon = commandsServer.write("get position/longitude-deg");
            double lat = commandsServer.write("get position/latitude-deg");
            ViewBag.FirstLon = lon;
            ViewBag.FirstLat = lat;
            return View();
        }

        [HttpGet]
        public ActionResult RouteSave(string arg1, int arg2, int arg3, int arg4, string arg5)
        {
            CommandsServer commandsServer = CommandsServer.getInstance();
            commandsServer.Ip = arg1;
            commandsServer.Port = arg2;
            ViewBag.UpdateRate = arg3;
            commandsServer.connect();
            double lon = commandsServer.write("get position/longitude-deg");
            double lat = commandsServer.write("get position/latitude-deg");
            ViewBag.FirstLon = lon;
            ViewBag.FirstLat = lat;
            ViewBag.TimeLimit = arg4;
            filePath = @"D:\" + arg5 + ".txt";
            TextWriter file = new StreamWriter(filePath);
            file.Close();
            return View();
        }

        [HttpPost]
        public string GetValuesFromServer()
        {
            CommandsServer commandsServer = CommandsServer.getInstance();
            commandsServer.connect();
            double lon = commandsServer.write("get position/longitude-deg");
            double lat = commandsServer.write("get position/latitude-deg");
            double rudder = commandsServer.write("get controls/flight/rudder");
            double throttle = commandsServer.write("get controls/engines/current-engine/throttle");
            return ToXML(lon.ToString() + " " + lat.ToString() + " " + rudder.ToString() + " " + throttle.ToString());
        }

        [HttpPost]
        public string SaveValuesFromServer()
        {
            CommandsServer commandsServer = CommandsServer.getInstance();
            commandsServer.connect();
            double lon = commandsServer.write("get position/longitude-deg");
            double lat = commandsServer.write("get position/latitude-deg");
            double rudder = commandsServer.write("get controls/flight/rudder");
            double throttle = commandsServer.write("get controls/engines/current-engine/throttle");
            string values = lon.ToString() + " " + lat.ToString() + " " + rudder.ToString() + " " + throttle.ToString();
            System.IO.File.AppendAllText(filePath, string.Format("{0}{1}", values, Environment.NewLine));
            return ToXML(values);
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
            writer.WriteElementString("Rud", temp[2]);
            writer.WriteElementString("Thr", temp[3]);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }

        public Boolean isIP(string addr)
        {
            IPAddress address;
            if (IPAddress.TryParse(addr, out address))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpGet]
        public void TwoArgs(string arg1, string arg2)
        {
            if (isIP(arg1))
            {
                LocationDisplay(arg1, Int32.Parse(arg2));
            }
        }
    }
}
