using System;
using System.Net.Sockets;
using System.Text;

namespace Ex3.Models.CommandsServer
{
    public class CommandsServer
    {
        public TcpClient client;
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
        private static volatile CommandsServer Instance;


        /// <summary>
        ///     Create a new singleton commands server.
        /// </summary>
        private CommandsServer() { }

        /// <summary>
        ///     Get an instance of the singleton.
        /// </summary>
        /// <returns> The singleton commands server. </returns> 
        public static CommandsServer getInstance()
        {
            if (Instance == null)
            {
                Instance = new CommandsServer();
            }
            return Instance;
        }

        /// <summary>
        ///     Create a new client and connect.
        /// </summary>
        public void connect()
        {
            // create a TCPClient object at the IP and port no.
            client = new TcpClient(ip, port);
        }

        /// <summary>
        ///     Add '\r\n' to the message so the server can read it correctly.
        /// </summary>
        /// <param name="message"></param> old message.
        /// <returns> updated message. </returns>
        public string prepareMessage(string message)
        {
            // if the message already ends with '\r\n', return it.
            if ((message[message.Length - 2] == '\r') && (message[message.Length - 1] == '\n'))
            {
                return message;
            }
            // if message ends with '\n', remove it.
            if (message[message.Length - 1] == '\n')
            {
                message = message.Remove(message.Length - 1);
            }
            message += "\r\n";
            return message;
        }

        /// <summary>
        ///     Extract double value from server.
        /// </summary>
        /// <param name="response"></param> server's response.
        /// <returns> extracted double value. </returns>
        public double extractValue(string response)
        {
            // if the message already ends with '\r\n', return it.
            if ((response[response.Length - 2] == '\r') && (response[response.Length - 1] == '\n'))
            {
                response = response.Remove(response.Length - 1);
                response = response.Remove(response.Length - 2);
            }
            // if message ends with '\n', remove it.
            if (response[response.Length - 1] == '\n')
            {
                response = response.Remove(response.Length - 1);
            }
            string[] words = response.Split('=');
            string val = words[1];
            val = val.Replace("'", "");
            val = val.Replace("(double)\r\n/>", "");
            val = val.Trim();
            return Convert.ToDouble(val);
        }


        /// <summary>
        ///     Receives a message from the user / manual components, prepares it for sending and then passes
        ///     it to the server.
        /// </summary>
        /// <param name="message"></param> The message from the user.
        public double write(string message)
        {
            NetworkStream nwStream = client.GetStream();
            message = prepareMessage(message);
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(message);
            // send the text.
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);
            // read back the text.
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            string response = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
            return extractValue(response);
        }
    }
}