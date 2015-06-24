using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using TicTacToeClient.Model;
using System.Windows.Media;

namespace TicTacToeClient.ViewModel {
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase {
        private Client _client;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService) {
            Port = 8000;
            IPAddress = "127.0.0.1";
            _client = new Client(IPAddress, Port);
            IsMyTurn = false;
            ConnectionButtonContent = "Connect";
            EnableControls = true;
            MyColor = new SolidColorBrush(Color.FromRgb(25, 25, 25));
            TheirColor = new SolidColorBrush(Color.FromRgb(25, 25, 25));
        }

        public void UpdateGameStatus(string status) {
            GameStatus = status;
            // send the status to the server
            _client.SendMessage("status:" + status);
            IsMyTurn = false;
        }

        private void LogMessage(string message) {
            DebugLog += "\n" + message;
            Log = message;
        }

        private void LogDebugMessage(string message) {
            DebugLog += "\n" + message;
        }

        private void ChatMessage(string message, string user = "") {
            ChatLog += "\n[" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "] "
                + user + ": " + message;
        }

        private void StartNewGame() {
            IsMyTurn = false;
            UpdateGameStatus("---------");
        }

        private void HandleServerMessage(string[] msgs) {
            foreach (var msg in msgs) {
                // the first part of message tells the type
                var splitMsg = msg.Split(':');
                switch (splitMsg[0]) {
                    case "status":
                        // game status changed, update
                        GameStatus = splitMsg[1];
                        break;
                    case "side":
                        // choose side
                        Side = splitMsg[1] == "x" ? "x" : "o";

                        //// change colors
                        //if (splitMsg[1] == "x") {
                        //    MyColor = new SolidColorBrush(Color.FromRgb(70, 110, 240));
                        //    TheirColor = new SolidColorBrush(Color.FromRgb(240, 110, 70));
                        //}
                        //else {
                        //    MyColor = new SolidColorBrush(Color.FromRgb(240, 110, 70));
                        //    TheirColor = new SolidColorBrush(Color.FromRgb(70, 110, 240));
                        //}

                        break;
                    case "turn":
                        if (splitMsg[1] == Side) {
                            IsMyTurn = true;
                            LogMessage("Your turn");
                        }
                        else {
                            LogMessage("Opponent's turn...");
                        }

                        break;
                    case "winner":
                        // winner was found, give client a point if won
                        if (splitMsg[1] == Side) {
                            MyPoints++;
                            LogDebugMessage("You won!");
                            ChatMessage("You won!");
                        }
                        else if (splitMsg[1] == "draw") {
                            LogDebugMessage("It's a draw!");
                            ChatMessage("It's a draw!");
                        }
                        else {
                            OpponentPoints++;
                            LogDebugMessage("You lost...");
                            ChatMessage("You lost...");
                        }
                        StartNewGame();
                        break;
                    case "client_disconnected":
                        LogDebugMessage("Opponent disconnected");
                        ChatMessage("Opponent disconnected");
                        IsMyTurn = false;
                        break;
                    case "chat":
                        var userName = splitMsg.Length == 3 ? splitMsg[2] : "";
                        ChatMessage(splitMsg[1], userName);
                        break;
                    default:
                        break;
                }
            }
        }

        private RelayCommand _connectCommand;

        /// <summary>
        /// Gets the ConnectCommand.
        /// </summary>
        public RelayCommand ConnectCommand {
            get {
                return _connectCommand
                    ?? (_connectCommand = new RelayCommand(
                                          () => {
                                              if (!_client.IsConnected) {
                                                  _client.ConnectionRefused += _client_ConnectionRefused;
                                                  _client.Connected += _client_Connected;
                                                  _client.Disconnected += _client_Disconnected;
                                                  _client.MessageReceived += _client_MessageReceived;
                                                  _client.MessageSent += _client_MessageSent;
                                                  _client.Connect();
                                              }
                                              else {
                                                  _client.Disconnect();
                                              }
                                          }));
            }
        }

        private RelayCommand _sayCommand;

        /// <summary>
        /// Gets the SayCommand.
        /// </summary>
        public RelayCommand SayCommand {
            get {
                return _sayCommand
                    ?? (_sayCommand = new RelayCommand(
                                          () => {
                                              if (ChatSay != "") {
                                                  _client.SendMessage("chat:" + ChatSay + ":" + Side);
                                                  ChatSay = "";
                                              }
                                          }));
            }
        }

        void _client_ConnectionRefused(object sender, ClientEventArgs e) {
            LogDebugMessage("Connection refused: " + e.Message);
        }

        void _client_MessageSent(object sender, ClientEventArgs e) {
            LogDebugMessage("Sent message: " + e.Message);
        }

        void _client_MessageReceived(object sender, ClientEventArgs e) {
            LogDebugMessage("Received message: " + String.Join("", e.Messages));
            HandleServerMessage(e.Messages);
        }

        void _client_Disconnected(object sender, ClientEventArgs e) {
            LogMessage("Disconnected.");
            ConnectionButtonContent = "Connect";
            EnableControls = true;
            IsMyTurn = false;
        }

        void _client_Connected(object sender, ClientEventArgs e) {
            LogMessage("Connected.");
            ConnectionButtonContent = "Disconnect";
            EnableControls = false;
        }

        private string _ipAddress;
        public string IPAddress {
            get {
                return _ipAddress;
            }
            set {
                if (_ipAddress != value) {
                    _ipAddress = value;
                    RaisePropertyChanged("IPAddress");
                }
            }
        }

        private int _port;
        public int Port {
            get {
                return _port;
            }
            set {
                if (_port != value) {
                    _port = value;
                    RaisePropertyChanged("Port");
                }
            }
        }

        private bool _enableControls;
        public bool EnableControls {
            get {
                return _enableControls;
            }
            set {
                if (_enableControls != value) {
                    _enableControls = value;
                    RaisePropertyChanged("EnableControls");
                }
            }
        }

        private string _connectionButtonContent;
        public string ConnectionButtonContent {
            get {
                return _connectionButtonContent;
            }
            set {
                if (_connectionButtonContent != value) {
                    _connectionButtonContent = value;
                    RaisePropertyChanged("ConnectionButtonContent");
                }
            }
        }

        private int _myPoints;
        public int MyPoints {
            get {
                return _myPoints;
            }
            set {
                if (_myPoints != value) {
                    _myPoints = value;
                    RaisePropertyChanged("MyPoints");
                }
            }
        }

        private int _opponentPoints;
        public int OpponentPoints {
            get {
                return _opponentPoints;
            }
            set {
                if (_opponentPoints != value) {
                    _opponentPoints = value;
                    RaisePropertyChanged("OpponentPoints");
                }
            }
        }

        private bool _isMyTurn;
        public bool IsMyTurn {
            get {
                return _isMyTurn;
            }
            set {
                if (_isMyTurn != value) {
                    _isMyTurn = value;
                    RaisePropertyChanged("IsMyTurn");
                }
            }
        }

        private string _side;
        public string Side {
            get {
                return _side;
            }
            set {
                if (_side != value) {
                    _side = value;
                    RaisePropertyChanged("Side");
                }
            }
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

        private SolidColorBrush _myColor;
        public SolidColorBrush MyColor {
            get {
                return _myColor;
            }
            set {
                if (_myColor != value) {
                    _myColor = value;
                    RaisePropertyChanged("MyColor");
                }
            }
        }

        private SolidColorBrush _theirColor;
        public SolidColorBrush TheirColor {
            get {
                return _theirColor;
            }
            set {
                if (_theirColor != value) {
                    _theirColor = value;
                    RaisePropertyChanged("TheirColor");
                }
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

        private string _debugLog;
        public string DebugLog {
            get {
                return _debugLog;
            }
            set {
                if (_debugLog != value) {
                    _debugLog = value;
                    RaisePropertyChanged("DebugLog");
                }
            }
        }
        
        private string _chatLog;
        public string ChatLog {
            get {
                return _chatLog;
            }
            set {
                if (_chatLog != value) {
                    _chatLog = value;
                    RaisePropertyChanged("ChatLog");
                }
            }
        }
        
        private string _chatSay;
        public string ChatSay {
            get {
                return _chatSay;
            }
            set {
                if (_chatSay != value) {
                    _chatSay = value;
                    RaisePropertyChanged("ChatSay");
                }
            }
        }

        internal void OnWindowClosing() {
            // dispose of the client
            _client.Dispose();
        }
    }
}