using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Discord.Commands;
using Discord.WebSocket;
using Renci.SshNet;
namespace JamBot
{
    class Server
    {
        public string host;
        public string port;
        public string managerID;
        public Server( string _host, string _port, string _managerID)
        {
            host = _host;
            port = _port;
            managerID = _managerID;
        }
    }  
    class Node
    {
        public int index;
        public string managerID;
        public Node( int _index, string _managerID)
        {
            index = _index;
            managerID = _managerID;
        }
    }
    class Functions
    {

        // Generate a Random Password
        internal static string GeneratePassword(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new();
            Random rnd = new();
            while (0 < length--)
            {
                res.Append(chars[rnd.Next(chars.Length)]);
            }
            return res.ToString();
        }
        internal static  bool IsPrivateMessage(SocketMessage msg)
        {
            return (msg.Channel is SocketDMChannel);
        }


        internal static Server[] GetServerList()
        {            
            string? path = ConfigurationManager.AppSettings["ServerList"];
            Server[] serverList;
            if (path != null)
            {
                string[] lines = File.ReadAllLines(path);
                serverList = new Server[lines.Length];

                for (int i = 0; i < lines.Length; i++)
                {
                    string[] tokens = lines[i].Split(";");
                    serverList[i] = new Server(tokens[0], tokens[1], tokens[2]);
                }
                return serverList;
            }
            return serverList = Array.Empty<Server>();
        }

        internal static string Connect(Server serverInfo, string message)
        {
            try
            {

                string server = serverInfo.host;
                Int32 port = Convert.ToInt32(serverInfo.port);
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer
                // connected to the same address as specified by the server, port
                // combination.
                //Int32 port = 13000;
                TcpClient client = new TcpClient(server, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                // Close everything.
                stream.Close();
                client.Close();

                return "OK " + responseData;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                return e.ToString();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                return e.ToString();
            }
        }
        // Adds Login and Password below the latest Login entry in the config file
        internal static async Task SFTP_addLogin(int serverId, string name, string pw)
        {      
            await Task.Run(() =>
            {

                using SftpClient client = new(ConfigurationManager.AppSettings["SSHHost"],  ConfigurationManager.AppSettings["SSHName"], ConfigurationManager.AppSettings["SSHPW"]);
              
                client.Connect();

                string path = ConfigurationManager.AppSettings["ServerPath" + serverId] + "/config.cfg";
                bool cnctd = client.IsConnected;
                if (cnctd)
                {
                    Console.WriteLine("Connected");
                }
                else
                {
                    Console.WriteLine("Connection failed");
                }
                // Rewrites the File, but adds a new line for the new Login under the last already existing login entry
                string[] arrLines = client.ReadAllLines(path);
                string[] lines = new string[arrLines.Length + 1];
                int index = 0;
                for (int i = 0; i < arrLines.Length; i++)
                {
                    // TODO: Filter Double usernames
                    if (index == 0 && i >= 30 && !arrLines[i].Contains("User"))
                    {
                        lines[i] = "User " + name + " " + pw + " CBTKRMHVP";
                        index = 1;
                    }
                    lines[i + index] = arrLines[i];
                }

                client.WriteAllLines(path, lines);
                client.Disconnect();
            });
        }


        // Gets Userinfo for all Ninjam Nodes on all Servers
        internal static string GetNodeStatuses(Server[] serverInfo)
        {
            string statusList = string.Empty;
            string output = string.Empty;
            for (int i = 0; i < serverInfo.Length; i++)
            {
                output = Connect(serverInfo[i], "Status");
                if (output.StartsWith("OK") || output.StartsWith("Dead"))
                {
                    statusList += output;
                }
            }
            return statusList;
        }

        // Return first Empty server
        internal static Node FindEmptyNode(Server[] serverInfo)
        {
            string status = GetNodeStatuses(serverInfo);
            string[] statuses = status.Split("\n");
            int index = Array.FindIndex(statuses, s => s.Contains(" users 0", StringComparison.OrdinalIgnoreCase));

            string managerID = statuses[index].Split(";")[2];
            Node node = new Node(index,managerID);
            return node;
        }

        /*
        internal static async Task GenCFG_AddLogin(string name, string pw)
        {
            await Task.Run(() =>
            {
                string path = "\\example.cfg";
                string[] arrLine = File.ReadAllLines(path);
                string[] lines = arrLine;
                Array.Resize(ref lines,lines.Length + 1);
                int index = 0;
                for (int i = 0; i < arrLine.Count(); i++)
                {
                    if (index == 0 && i >= 30 && !arrLine[i].Contains("User"))
                    {
                        lines[i] = "User " + name + " " + pw + " CBTKRMHVP";
                        index=1;
                    }
                    lines[i+index] = arrLine[i];
                }
                File.WriteAllLines(path, lines);
            });
        }
*/
    }
}
