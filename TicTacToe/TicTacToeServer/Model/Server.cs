using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;

namespace TicTacToeServer.Model {
    public class Server {
        public bool Started { get; set; }
        public int MaxNumberOfClients { get; set; }
        public List<TcpClient> Clients;

        public int ClientsConnected { get { return Clients.Count; } }
        
        private TcpListener _tcpListener;
        private Thread _listenThread;

        public Server(int maxClients = 2) {
            Started = false;
            MaxNumberOfClients = maxClients;

            Clients = new List<TcpClient>();
        }

        public void Start(string ip, int port) {
            _tcpListener = new TcpListener(IPAddress.Parse(ip), port);
            _listenThread = new Thread(new ThreadStart(ListenForClients));
            _listenThread.Start();
            Started = true;
        }

        public void Stop() {
            if (_listenThread == null) return;
            Started = false;
            _listenThread.Abort();
            _tcpListener.Stop();
            foreach (TcpClient c in Clients) {
                c.Close();                
            }
            Clients = new List<TcpClient>();
        }

        public void SendMessageToClient(TcpClient client, string message) {
            var encoder = new ASCIIEncoding();
            NetworkStream clientStream = client.GetStream();

            // put a divisor at the end of message to help parsing the final message
            message = message + '|';

            byte[] buffer = encoder.GetBytes(message);

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();           
        }

        public void SendMessageToAllClients(string message) {
            if (Clients == null || Clients.Count < 1) return;

            foreach (var client in Clients) {
                SendMessageToClient(client, message);
            }
            OnMessageSent(new ClientEventArgs() { Message = message });
        }

        private void ListenForClients() {
            _tcpListener.Start();

            while (true) {
                if (ClientsConnected == MaxNumberOfClients) continue;
                //blocks until a client has connected to the server
                TcpClient client = _tcpListener.AcceptTcpClient();
                Clients.Add(client);
                OnClientConnected(new ClientEventArgs() { Client = client, ClientName = Clients.IndexOf(client).ToString() });

                //create a thread to handle communication with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        private void HandleClientComm(object client) {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true) {
                bytesRead = 0;

                try {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0) {
                    //the client has disconnected from the server
                    OnClientDisconnected(new ClientEventArgs() { Client = tcpClient, ClientName = Clients.IndexOf(tcpClient).ToString() });
                    Clients.Remove(tcpClient);
                    break;
                }

                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();

                var bufferincmessage = encoder.GetString(message, 0, bytesRead);

                OnClientMessage(new ClientEventArgs() { Client = tcpClient, ClientName = Clients.IndexOf(tcpClient).ToString(), Message = bufferincmessage });
            }
        }

        protected virtual void OnClientConnected(ClientEventArgs e) {
            EventHandler<ClientEventArgs> handler = ClientConnected;
            if (handler != null) {
                handler(this, e);
            }
        }
        public event EventHandler<ClientEventArgs> ClientConnected;

        protected virtual void OnClientDisconnected(ClientEventArgs e) {
            EventHandler<ClientEventArgs> handler = ClientDisconnected;
            if (handler != null) {
                handler(this, e);
            }
        }
        public event EventHandler<ClientEventArgs> ClientDisconnected;

        protected virtual void OnMessageSent(ClientEventArgs e) {
            EventHandler<ClientEventArgs> handler = MessageSent;
            if (handler != null) {
                handler(this, e);
            }
        }
        public event EventHandler<ClientEventArgs> MessageSent;

        protected virtual void OnClientMessage(ClientEventArgs e) {
            EventHandler<ClientEventArgs> handler = ClientMessage;
            if (handler != null) {
                handler(this, e);
            }
        }
        public event EventHandler<ClientEventArgs> ClientMessage;
    }

    public class ClientEventArgs : EventArgs {
        public string ClientName { get; set; }
        public string Message { get; set; }
        public string[] Messages { get; set; }
        public TcpClient Client { get; set; }
    }
}
