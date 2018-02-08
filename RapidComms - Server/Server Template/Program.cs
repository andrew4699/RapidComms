using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Server_Template
{
    class Program
    {
        static void Main(string[] args)
        {
            const int SERVER_PORT = 8889;

            List<Client> clientList = new List<Client>();

            TcpListener listener = new TcpListener(IPAddress.Any, SERVER_PORT);
            //TcpListener listener = new TcpListener(IPAddress.Loopback, SERVER_PORT);
            TcpClient clientSocket = default(TcpClient);

            int clientNumber = 0;

            listener.Start();
            Console.WriteLine(">> Server started");

            while(true)
            {
                clientNumber++;

                clientSocket = listener.AcceptTcpClient();
                Console.WriteLine(">> A new client has connected (#" + clientNumber + ")");

                Client newClient = new Client(clientSocket, clientNumber);
                Server.clientList.Add(newClient);
            }

            clientSocket.Close();
            listener.Stop();

            Console.WriteLine(">> Server stopped");
            Console.ReadLine();
        }
    }
}
