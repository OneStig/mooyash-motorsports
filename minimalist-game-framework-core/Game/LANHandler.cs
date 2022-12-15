using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.ComponentModel;

using Mooyash.Modules;

namespace Mooyash.Services
{
    public static class LANHandler
    {
        private static TcpListener host;
        private static TcpClient client;
        private static BackgroundWorker MessageReceiver;
        private static Socket socket;

        private static System.Net.IPAddress hostIP;

        private static int port = 5732;

        public static void init()
        {
            MessageReceiver = new BackgroundWorker();
            MessageReceiver.DoWork += CheckSocket;
        }

        private static void CheckSocket(object sender, DoWorkEventArgs a)
        {
            byte[] buffer = new byte[1];
            socket.Receive(buffer);

            Console.WriteLine(buffer[0]);
        }

        private static void SocketSend(byte[] payload)
        {
            socket.Send(payload);
        }

        public static bool openServer()
        {
            try
            {
                hostIP = System.Net.IPAddress.Any;
                host = new TcpListener(hostIP, port);
                host.Start();

                socket = host.AcceptSocket();

                Console.WriteLine(hostIP.ToString());

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool connectClient(string ip)
        {
            try
            {
                client = new TcpClient(ip, port);
                socket = client.Client;
                // MessageReceiver.RunWorkerAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

