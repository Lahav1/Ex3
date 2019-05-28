using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Ex3.Models.InfoServer
{
    public class InfoServer
    {
        private bool shouldStop;
        public bool ShouldStop
        {
            get => shouldStop;
            set => shouldStop = value;
        }
        private string ip;
        public string Ip
        {
            get => ip;
            set => ip = value;
        }
        private int port;
        public int Port
        {
            get => port;
            set => port = value;
        }
        public TcpClient client;
        public TcpListener listener;
        private double lon;
        public double Lon
        {
            get
            {
                return values["longitude"];
            }
            set
            {
                lon = values["longitude"];
            }
        }
        private double lat;
        public double Lat
        {
            get
            {
                return values["latitude"];
            }
            set
            {
                lon = values["latitude"];
            }
        }

        private Dictionary<string, double> values;
        private static volatile InfoServer Instance;

        /// <summary>
        ///     Create a new singleton commands server.
        /// </summary>
        private InfoServer()
        {
            values = new Dictionary<string, double>();
            // init the dictionary keys.
            values.Add("longitude", 0);
            values.Add("latitude", 0);
            values.Add("throttle", 0);
            values.Add("rudder", 0);
            // init loop term. will be changed to true after "disconnect" is pressed.
            shouldStop = false;
        }

        /// <summary>
        ///     Get an instance of the singleton.
        /// </summary>
        /// <returns> The singleton commands InfoServer. </returns> 
        public static InfoServer getInstance()
        {
            if (Instance == null)
            {
                Instance = new InfoServer();
            }
            return Instance;
        }

        /// <summary>
        ///     Get an instance of the singleton.
        /// </summary>
        /// <param> name="data" - the data string received from server. </param> 
        void updateValues(string data)
        {
            double[] currentValues = separateValues(data);
            // if something went wrong with the data buffering, skip the update.
            if (currentValues.Length != 25)
            {
                return;
            }
            // update the relevant values.
            values["longitude"] = currentValues[0];
            values["latitude"] = currentValues[1];
            values["throttle"] = currentValues[23];
            values["rudder"] = currentValues[21];
        }

        /// <summary>
        ///     Separates the data string from the server to an array of double values.
        /// </summary>
        /// <param name="data"></param> data string from server.
        /// <returns> array of 25 double values. </returns>
        double[] separateValues(string data)
        {
            double[] currentValues = new double[25];
            int i;
            // split the buffer by "," char.
            string[] words = data.Split(',');
            if (words[24][words[24].Length - 1] != '\n')
            {
                return currentValues;
            }
            // convert each string to a double.
            for (i = 0; i <= 24; i++)
            {
                currentValues[i] = Convert.ToDouble(words[i]);
            }
            return currentValues;
        }

        /// <summary>
        ///     prints the current values to console.
        /// </summary>
        void printValues()
        {
            foreach (var value in values)
            {
                Console.Write(value.Key + ": " + value.Value + "   ");
            }
            Console.WriteLine("\r\n");
        }

        /// <summary>
        ///     create a new tcp listener.
        /// </summary>
        public void connect()
        {
            IPAddress localAdd = IPAddress.Parse(ip);
            listener = new TcpListener(localAdd, port);
            // start listening for a connection.
            listener.Start();
            // incoming client connected.
            client = listener.AcceptTcpClient();
            // start reading from server in a new thread.
            Thread t = new Thread(read);
            t.Start();
        }

        /// <summary>
        ///     Read the values from server 
        /// </summary>
        public void read()
        {
            // after connection is achieved, while shouldStop is set to "false", keep reading.
            while (!shouldStop)
            {
                // get the incoming data through a network stream.
                NetworkStream nwStream = client.GetStream();
                byte[] buffer = new byte[client.ReceiveBufferSize];
                // read incoming stream---
                int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
                // convert the data received into a string.
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                // dataReceived = getData(dataReceived);
                updateValues(dataReceived);
                printValues();
                // if data is invalid skip update.
                Thread.Sleep(100);
            }
        }
    }
}