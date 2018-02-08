using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server_Template
{
    class Server
    {
        public static List<Client> clientList = new List<Client>();

        public static void sendMessageToAll(int type, string message, Client exclude = null)
        {
            for(int clientIndex = 0; clientIndex < clientList.Count; clientIndex++)
            {
                if (clientList[clientIndex] != exclude)
                {
                    //Console.WriteLine("sending (" + message + ") to client #" + ((Client)clientList[clientIndex]).clientNumber);
                    ((Client)clientList[clientIndex]).addMessage(type, message);
                }
            }
        }
    }
}
