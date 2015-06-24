using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TicTacToeClient.Views {
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl {
        public string GameStatus {
            get { return (string)GetValue(GameStatusProperty); }
            set { SetValue(GameStatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GameStatus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GameStatusProperty =
            DependencyProperty.Register("GameStatus", typeof(string), typeof(GameView), new PropertyMetadata(null, new PropertyChangedCallback(SetGameStatus)));

        public GameView() {
            InitializeComponent();
        }

        private void gameGrid_Loaded(object sender, RoutedEventArgs e) {
            // make buttons
            for (int row = 0; row <= 2; row++) {
                for (int col = 0; col <= 2; col++) {
                    var btn = new Button() {
                        Name = "btn_" + row + "" + col,
                        VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                        FontSize = 60
                    };

                    btn.Click += btn_Click;

                    Grid.SetColumn(btn, col);
                    Grid.SetRow(btn, row);

                    gameGrid.Children.Add(btn);
                }
            }
        }

        void btn_Click(object sender, RoutedEventArgs e) {
            var vm = this.DataContext as TicTacToeClient.ViewModel.MainViewModel;

            var gStatus = GameStatus != null ? GameStatus.ToCharArray() : "---------".ToCharArray();
            if (gStatus.Length != 9) return;

            Button btn = sender as Button;
            //if (btn.Content != null) return;

            // check whose turn it is from status
            var turnX = (gStatus.Where(s => !s.Equals('-')).Count() % 2) == 0;

            // update status and send it to gameviewmodel
            int col = Int32.Parse(btn.Name[btn.Name.Length - 1].ToString());
            int row = Int32.Parse(btn.Name[btn.Name.Length - 2].ToString());
            gStatus[col + row * 3] = turnX ? 'x' : 'o';

            vm.UpdateGameStatus(new string(gStatus));
        }

        private void SetGameStatus(char[] gameStatus) {
            if (gameGrid.Children.Count < 1) return;
            if (gameGrid.Children.Count != gameStatus.Length) return;

            for (int i = 0; i < gameGrid.Children.Count; i++) {
                var btn = gameGrid.Children[i] as Button;
                switch (gameStatus[i]) {
                    case 'x':
                        btn.Content = 'X';
                        btn.Foreground = new SolidColorBrush(Color.FromRgb(70, 110, 240));
                        btn.IsEnabled = false;
                        break;
                    case 'o':
                        btn.Content = 'O';
                        btn.Foreground = new SolidColorBrush(Color.FromRgb(240, 110, 70));
                        btn.IsEnabled = false;
                        break;
                    default:
                        btn.Content = null;
                        btn.IsEnabled = true;
                        break;
                }
            }
        }

        private static void SetGameStatus(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs) {
            if (eventArgs.NewValue == null) return;

            var view = dependencyObject as GameView;
            view.SetGameStatus(eventArgs.NewValue.ToString().ToCharArray());
        }
    }
}
