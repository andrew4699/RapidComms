using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace Server_Template
{
    class Client
    {
        TcpClient clientSocket;
        public int clientNumber;

        public class Message
        {
            public int type;
            public string message;

            public Message(int mType, string mMessage)
            {
                this.type = mType;
                this.message = mMessage;
            }
        }

        List<Message> messageQueue = new List<Message>();

        public Client(TcpClient clientSocket, int clientNumber)
        {
            this.clientSocket = clientSocket;
            this.clientNumber = clientNumber;

            Thread clientThread = new Thread(doChat);
            clientThread.Start();
        }

        private void onReceiveClientMessage(int type, string message)
        {
            Console.WriteLine(">> Received data from client #" + clientNumber + ", sending to other clients");

            if (type == Messages.RECEIVED)
            {
                messageQueue.RemoveAt(0);

                if (messageQueue.Count > 0)
                {
                    sendMessage(messageQueue[0].type, messageQueue[0].message);
                }
            }
            else
            {
                Server.sendMessageToAll(type, message, this);
                sendMessage(Messages.RECEIVED);
            }
        }

        public void addMessage(int type, string message)
        {
            messageQueue.Add(new Message(type, message));

            if (messageQueue.Count == 1)
            {
                sendMessage(messageQueue[0].type, messageQueue[0].message);
            }
        }

        public void sendMessage(int type, string message = "")
        {
            Byte[] sendBytes = null;

            NetworkStream networkStream = clientSocket.GetStream();
            sendBytes = Encoding.ASCII.GetBytes(type + "|" + message + "$");

            networkStream.Write(sendBytes, 0, sendBytes.Length);
            networkStream.Flush();
        }

        // Internal

        private void doChat()
        {
            byte[] bytesFrom = new byte[2000000];
            string dataFromClient = null;

            NetworkStream networkStream = clientSocket.GetStream();

            while (clientSocket.Connected)
            {
                try
                {
                    networkStream.Read(bytesFrom, 0, 2000000);

                    dataFromClient = Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                    // Received message (dataFromClient)

                    string[] dataSplit = dataFromClient.Split(new char[] { '|' }, 2);

                    if (dataSplit[1] == ";disconnect")
                    {
                        disconnect();
                        break;
                    }

                    onReceiveClientMessage(Convert.ToInt32(dataSplit[0]), dataSplit[1]);
                }
                catch/*(Exception ex)*/
                {
                    disconnect();
                }
            }
        }

        private void disconnect()
        {
            Console.WriteLine(">> Client #" + clientNumber + " has disconnected");
            clientSocket.Close();

            Server.clientList.Remove(this);
        }
    }
}
