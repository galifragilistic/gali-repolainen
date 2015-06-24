using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using TicTacToeServer.Model;
using System.Linq;

namespace TicTacToeServer.ViewModel {
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase {
        private Server _server;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService) {
            ServerButtonContent = "Start Server";

            ServerIP = "127.0.0.1";
            ServerPort = 8000;

            _server = new Server(2);
            _server.ClientConnected += _server_ClientConnected;
            _server.ClientDisconnected += _server_ClientDisconnected;
            _server.ClientMessage += _server_ClientMessage;
            _server.MessageSent += _server_SentMessage;
        }

        void _server_SentMessage(object sender, ClientEventArgs e) {
            LogMessage("Sent: " + e.Message);
        }

        void _server_ClientMessage(object sender, ClientEventArgs e) {
            LogMessage("Client " + e.ClientName + " sent: '" + e.Message + "'");
            HandleClientMessage(e.Message);
        }

        void _server_ClientDisconnected(object sender, ClientEventArgs e) {
            LogMessage("Client " + e.ClientName + " disconnected.");
            _server.SendMessageToAllClients("client_disconnected");
        }

        void _server_ClientConnected(object sender, ClientEventArgs e) {
            LogMessage("Client " + e.ClientName + " connected.");

            if (_server.ClientsConnected == 2) {
                // assign sides
                _server.SendMessageToClient(_server.Clients[0], "side:x");
                _server.SendMessageToClient(_server.Clients[1], "side:y");

                _server.SendMessageToAllClients("status:---------");
                // game begins with X's turn
                _server.SendMessageToAllClients("turn:x");
            }
        }

        private string _serverButtonContent;
        public string ServerButtonContent {
            get {
                return _serverButtonContent;
            }
            set {
                if (_serverButtonContent != value) {
                    _serverButtonContent = value;
                    RaisePropertyChanged("ServerButtonContent");
                }
            }
        }

        private string _dataText;
        public string DataText {
            get {
                return _dataText;
            }
            set {
                if (_dataText != value) {
                    _dataText = value;
                    RaisePropertyChanged("DataText");
                }
            }
        }

        private RelayCommand _sendDataCommand;
        public RelayCommand SendDataCommand {
            get {
                return _sendDataCommand
                    ?? (_sendDataCommand = new RelayCommand(
                                          () => {
                                              if (DataText != "") {
                                                  _server.SendMessageToAllClients(DataText);
                                              }
                                          }));
            }
        }

        private RelayCommand _startServerCommand;

        /// <summary>
        /// Gets the StartServerCommand.
        /// </summary>
        public RelayCommand StartServerCommand {
            get {
                return _startServerCommand
                    ?? (_startServerCommand = new RelayCommand(
                                          () => {
                                              if (!_server.Started) {
                                                  // start accepting connections
                                                  _server.Start(ServerIP, ServerPort);
                                                  LogMessage("Started to listen port " + ServerPort + ".");
                                                  ServerButtonContent = "Stop Server";
                                              }
                                              else {
                                                  _server.Stop();
                                                  LogMessage("Server stopped.");
                                                  ServerButtonContent = "Start Server";
                                              }
                                          }));
            }
        }

        private string _log;
        public string Log {
            get {
                return _log;
            }
            set {
                if (_log != value) {
                    _log = value;
                    RaisePropertyChanged("Log");
                }
            }
        }

        private void LogMessage(string msg) {
            Log += "\n[" + DateTime.Now + "]: " + msg;
        }

        private string _gameStatus;
        public string GameStatus {
            get {
                return _gameStatus;
            }
            set {
                if (_gameStatus != value) {
                    _gameStatus = value;
                    RaisePropertyChanged("GameStatus");
                }
            }
        }
        
        private string _serverIP;
        public string ServerIP {
            get {
                return _serverIP;
            }
            set {
                if (_serverIP != value) {
                    _serverIP = value;
                    RaisePropertyChanged("ServerIP");
                }
            }
        }
        
        private int _serverPort;
        public int ServerPort {
            get {
                return _serverPort;
            }
            set {
                if (_serverPort != value) {
                    _serverPort = value;
                    RaisePropertyChanged("ServerPort");
                }
            }
        }

        private void HandleClientMessage(string msg) {
            if (msg == null) return;
            var splitMsg = msg.Split(':');
            switch (splitMsg[0]) {
                case "status":
                    if (splitMsg[1] == GameStatus) return;

                    GameStatus = splitMsg[1];
                    var winner = CheckWinningCondition(GameStatus);
                    if (winner == "none") {
                        // send status to all clients
                        _server.SendMessageToAllClients(msg);
                        // tell a client it's their turn
                        _server.SendMessageToAllClients("turn:" + GetTurn(GameStatus));
                    }
                    else {
                        // send status to all clients
                        _server.SendMessageToAllClients(msg);
                        _server.SendMessageToAllClients("winner:" + winner);
                    }
                    break;
                case "chat":
                    _server.SendMessageToAllClients(msg);
                    break;
                default:
                    break;
            }
        }

        private string CheckWinningCondition(string status) {
            int x = 0, o = 0;
            char p;

            // horizontal
            for (int i = 0; i <= 6; i += 3) {
                for (int j = 0; j < 3; j++) {
                    p = status[i + j];
                    if (p == 'x') x++;
                    else if (p == 'o') o++;

                    if (x == 3) return "x";
                    else if (o == 3) return "o";
                }
                x = 0;
                o = 0;
            }
            // vertical
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    p = status[i + j * 3];
                    if (p == 'x') x++;
                    else if (p == 'o') o++;

                    if (x == 3) return "x";
                    else if (o == 3) return "o";
                }
                x = 0;
                o = 0;
            }

            // diagonal
            for (int i = 0; i < 9; i += 4) {
                p = status[i];
                if (p == 'x') x++;
                else if (p == 'o') o++;

                if (x == 3) return "x";
                else if (o == 3) return "o";
            }
            x = 0;
            o = 0;
            for (int i = 2; i < 8; i += 2) {
                p = status[i];
                if (p == 'x') x++;
                else if (p == 'o') o++;

                if (x == 3) return "x";
                else if (o == 3) return "o";
            }

            if (!status.Any(c => c == '-')) return "draw";

            return "none";
        }

        private string GetTurn(string status) {
            // check whose turn it is from status
            var gstatus = status.ToCharArray();
            var turnX = (gstatus.Where(s => !s.Equals('-')).Count() % 2) == 0;

            var turn = turnX ? "x" : "o";
            return turn;
        }

        internal void OnWindowClosing() {
            _server.Stop();
        }
    }
}