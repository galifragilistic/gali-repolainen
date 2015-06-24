using System.Windows;
using TicTacToeClient.ViewModel;

namespace TicTacToeClient {
    /// <summary>
    /// This application's main window.
    /// </summary>
    public partial class MainWindow : Window {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow() {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void Label_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            //popDebug.IsOpen = true;
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            //scroll.ScrollToBottom();
        }

        private void btnChat_Click(object sender, RoutedEventArgs e) {
            popChat.IsOpen = !popChat.IsOpen;
            btnChat.Content = popChat.IsOpen ? "Chat <<" : "Chat >>";
        }
    }
}