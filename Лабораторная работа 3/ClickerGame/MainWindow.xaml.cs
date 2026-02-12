using System.Windows;
using System.Windows.Input;

namespace ClickerGame
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
        }

        private void EnemyIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.Game.ClickEnemy();
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Game.RepeatEnemy();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Game.NextEnemy();
        }

        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Game.Upgrade();
        }
    }
}