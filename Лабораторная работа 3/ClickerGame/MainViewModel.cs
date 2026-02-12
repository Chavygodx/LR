using System.ComponentModel;
using System.Windows.Media;

namespace ClickerGame
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private GameManager _game;

        public GameManager Game => _game;
        public string EnemyName => _game.CurrentEnemy?.Name ?? "—";
        public string EnemyHitPoints => _game.CurrentEnemy?.HitPoints.ToString() ?? "0";
        public string EnemyGoldReward => _game.CurrentEnemy?.GoldReward.ToString() ?? "0";
        public string PlayerGold => _game.Player.Gold.ToString();
        public string PlayerDamage => _game.Player.Damage.ToString();
        public string PlayerLevel => _game.Player.Level.ToString();
        public string UpgradeCost => _game.Player.UpgradeCost.ToString();
        public ImageSource EnemyIcon => _game.CurrentEnemy?.Icon;

        public MainViewModel()
        {
            _game = new GameManager();
            _game.LoadIcons("icons");
            _game.LoadTemplates("enemies.json");
            _game.SpawnEnemy();
            _game.StateChanged += RefreshAllProperties;
        }

        private void RefreshAllProperties()
        {
            OnPropertyChanged(nameof(EnemyName));
            OnPropertyChanged(nameof(EnemyHitPoints));
            OnPropertyChanged(nameof(EnemyGoldReward));
            OnPropertyChanged(nameof(PlayerGold));
            OnPropertyChanged(nameof(PlayerDamage));
            OnPropertyChanged(nameof(PlayerLevel));
            OnPropertyChanged(nameof(UpgradeCost));
            OnPropertyChanged(nameof(EnemyIcon));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}