using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace TicTacToeClient.Model {
    class Client : IDisposable{
        public IPAddress ServerIP{ get; set; }
        public Int32 ServerPort { get; set; }
        public bool IsConnected { get; set; }

        private TcpClient _clientSocket;
        private NetworkStream _serverStream;
        private Thread _clientThread;

        public Client(string ipAddress, int port) {
            ServerIP = IPAddress.Parse(ipAddress);
            ServerPort = port;
        }

        public void Connect() {
            _clientSocket = new TcpClient();
            _clientThread = new Thread(new ThreadStart(ConnectToServer));
            _clientThread.Start();
        }

        public void Disconnect() {
            if (_clientSocket != null && _clientSocket.Connected) {
                _clientSocket.GetStream().Close();
                _clientSocket.Close();
            }
            if (_clientThread != null && _clientThread.IsAlive) _clientThread.Abort();
        }

        public void SendMessage(string message) {
            _serverStream = _clientSocket.GetStream();
            byte[] outStream = Encoding.ASCII.GetBytes(message);
            _serverStream.Write(outStream, 0, outStream.Length);
            _serverStream.Flush();
            OnMessageSent(new ClientEventArgs() { Message = message });
        }

        public void Dispose() {
            this.Disconnect();
        }

        private void ConnectToServer() {
            try {
                _clientSocket.Connect(ServerIP, ServerPort);
                OnConnected(new ClientEventArgs());
            }
            catch (SocketException e) {
                OnConnectionRefused(new ClientEventArgs() { Message = e.Message });
                return;
            }
            _serverStream = _clientSocket.GetStream();
            byte[] message = new byte[4096];
            int bytesRead;

            while (true) {
                bytesRead = 0;

                try {
                    //blocks until a client sends a message
                    bytesRead = _serverStream.Read(message, 0, 4096);
                }
                catch {
                    //a socket error has occured
                    OnDisconnected(new ClientEventArgs());
                    break;
                }

                if (bytesRead == 0) {
                    //the client has disconnected from the server
                    OnDisconnected(new ClientEventArgs());
                    break;
                }

                // message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();

                var bufferincmessage = encoder.GetString(message, 0, bytesRead);

                // there may be several messages on one string, so split it to an array
                var messages = bufferincmessage.Split('|');

                OnMessageReceived(new ClientEventArgs() { Messages = messages });
            }
        }

        protected virtual void OnConnectionRefused(ClientEventArgs e) {
            EventHandler<ClientEventArgs> handler = ConnectionRefused;
            if (handler != null) {
                handler(this, e);
            }
        }
        public event EventHandler<ClientEventArgs> ConnectionRefused;

        protected virtual void OnConnected(ClientEventArgs e) {
            EventHandler<ClientEventArgs> handler = Connected;
            if (handler != null) {
                handler(this, e);
            }
            this.IsConnected = true;
        }
        public event EventHandler<ClientEventArgs> Connected;

        protected virtual void OnDisconnected(ClientEventArgs e) {
            EventHandler<ClientEventArgs> handler = Disconnected;
            if (handler != null) {
                handler(this, e);
            }
            this.IsConnected = false;
        }
        public event EventHandler<ClientEventArgs> Disconnected;

        protected virtual void OnMessageSent(ClientEventArgs e) {
            EventHandler<ClientEventArgs> handler = MessageSent;
            if (handler != null) {
                handler(this, e);
            }
        }
        public event EventHandler<ClientEventArgs> MessageSent;

        protected virtual void OnMessageReceived(ClientEventArgs e) {
            EventHandler<ClientEventArgs> handler = MessageReceived;
            if (handler != null) {
                handler(this, e);
            }
        }
        public event EventHandler<ClientEventArgs> MessageReceived;
    }

    public class ClientEventArgs : EventArgs {
        public string ClientName { get; set; }
        public string Message { get; set; }
        public string[] Messages { get; set; }
        public TcpClient Client { get; set; }
    }
}
