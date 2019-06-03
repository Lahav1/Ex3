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
        private static List<string> lineList;
        public const string SCENARIO_FILE = "~/App_Data/{0}.txt";
        private static string filePath;

        /// <summary>
        ///     Controls the single location display screen.
        /// </summary>
        /// <param name="arg1"> server IP. </param>
        /// <param name="arg2"> server port. </param>
        /// <returns> view of single location display screen. </returns>
        [HttpGet]
        public ActionResult LocationDisplay(string arg1, int arg2)
        {
            // connect to server.
            CommandsServer commandsServer = CommandsServer.getInstance();
            commandsServer.Ip = arg1;
            commandsServer.Port = arg2;
            commandsServer.connect();
            // request values of lon, lat and pass through viewbag. 
            double lon = commandsServer.write("get position/longitude-deg");
            double lat = commandsServer.write("get position/latitude-deg");
            ViewBag.Lon = lon;
            ViewBag.Lat = lat;
            return View("~/Views/Map/LocationDisplay.cshtml");
        }

        [HttpGet]
        public ActionResult RouteDisplay(string arg1, int arg2, int arg3)
        {
            // connect to server.
            CommandsServer commandsServer = CommandsServer.getInstance();
            commandsServer.Ip = arg1;
            commandsServer.Port = arg2;
            ViewBag.UpdateRate = arg3;
            commandsServer.connect();
            // request initial lon, lat and pass through viewbag.
            double lon = commandsServer.write("get position/longitude-deg");
            double lat = commandsServer.write("get position/latitude-deg");
            ViewBag.FirstLon = lon;
            ViewBag.FirstLat = lat;
            return View();
        }

        [HttpGet]
        public ActionResult RouteSave(string arg1, int arg2, int arg3, int arg4, string arg5)
        {
            // connect to server.
            CommandsServer commandsServer = CommandsServer.getInstance();
            commandsServer.Ip = arg1;
            commandsServer.Port = arg2;
            commandsServer.connect();
            // pass update rate through viewbag.
            ViewBag.UpdateRate = arg3;
            ViewBag.TimeLimit = arg4;
            // request initial lon, lat and pass through viewbag.
            double lon = commandsServer.write("get position/longitude-deg");
            double lat = commandsServer.write("get position/latitude-deg");
            ViewBag.FirstLon = lon;
            ViewBag.FirstLat = lat;
            // create a txt file to save the sampled location on and set it as a data member to access later.
            filePath = System.Web.HttpContext.Current.Server.MapPath(String.Format(SCENARIO_FILE, arg5));
            TextWriter file = new StreamWriter(filePath);
            file.Close();
            return View();
        }

        [HttpPost]
        public string GetValuesFromServer()
        {
            // connect to server.
            CommandsServer commandsServer = CommandsServer.getInstance();
            commandsServer.connect();
            // request lon, lat, rudder and throttle values from server.
            double lon = commandsServer.write("get position/longitude-deg");
            double lat = commandsServer.write("get position/latitude-deg");
            double rudder = commandsServer.write("get controls/flight/rudder");
            double throttle = commandsServer.write("get controls/engines/current-engine/throttle");
            // create a string of the values separated by spaces and return it as an XML.
            return ToXML(lon.ToString() + " " + lat.ToString() + " " + rudder.ToString() + " " + throttle.ToString());
        }

        [HttpPost]
        public string SaveValuesFromServer()
        {
            // connect to server.
            CommandsServer commandsServer = CommandsServer.getInstance();
            commandsServer.connect();
            // request lon, lat, rudder and throttle values from server.
            double lon = commandsServer.write("get position/longitude-deg");
            double lat = commandsServer.write("get position/latitude-deg");
            double rudder = commandsServer.write("get controls/flight/rudder");
            double throttle = commandsServer.write("get controls/engines/current-engine/throttle");
            // create a string of the values separated by spaces.
            string values = lon.ToString() + " " + lat.ToString() + " " + rudder.ToString() + " " + throttle.ToString();
            // save the values string on the txt file.
            System.IO.File.AppendAllText(filePath, string.Format("{0}{1}", values, Environment.NewLine));
            // return the values string as an XML.
            return ToXML(values);
        }

        public string ToXML(string coordinates)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("coordinates");
            // split the string by spaces.
            string[] temp = coordinates.Split(' ');
            // fill values to fields in XML.
            writer.WriteElementString("Lon", temp[0]);
            writer.WriteElementString("Lat", temp[1]);
            writer.WriteElementString("Rud", temp[2]);
            writer.WriteElementString("Thr", temp[3]);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            // stringify the XML.
            return sb.ToString();
        }

        public bool isIP(string addr)
        {
            // check if the string represents an ip address.
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
        public ActionResult LoadRoute(string arg1, int arg2)
        {
            // turn the txt file to list of strings.
            filePath = System.Web.HttpContext.Current.Server.MapPath(String.Format(SCENARIO_FILE, arg1));
            var lineArr = System.IO.File.ReadAllLines(filePath);
            lineList = new List<string>(lineArr);
            // pass update rate through viewbag.
            ViewBag.UpdateRate = arg2;
            // split line by spaces and pass the initial values through viewbag, to draw the starting point.
            var initialValues = lineList.ElementAt(0).Split(' ');
            double lon = Double.Parse(initialValues[0]);
            double lat = Double.Parse(initialValues[1]);
            ViewBag.FirstLon = lon;
            ViewBag.FirstLat = lat;
            // remove the first line.
            lineList.Remove(lineList.ElementAt(0));
            return View("~/Views/Map/LoadRoute.cshtml");
        }

        [HttpPost]
        public string GetValuesFromFile()
        {
            // create an xml from first line. 
            string lineXML = ToXML(lineList.ElementAt(0));
            // if there is more than 1 line, remove the first line.
            if (lineList.Count > 1)
            {
                lineList.Remove(lineList.ElementAt(0));
            }
            return lineXML;
        }

        [HttpGet]
        public ActionResult TwoArgs(string arg1, string arg2)
        {
            // if first arg is an ip address, return location display view.
            if (isIP(arg1))
            {
                return LocationDisplay(arg1, Int32.Parse(arg2));
            // else, return the load route from file view. 
            } else
            {
                return LoadRoute(arg1, Int32.Parse(arg2));
            }
        }
    }
}
