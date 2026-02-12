using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClickerGame
{
    public class GameManager
    {
        private List<CEnemyTemplate> _templates;
        private Dictionary<string, ImageSource> _icons;
        private Random _random;

        public CPlayer Player { get; private set; }
        public CEnemy CurrentEnemy { get; private set; }

        public event Action StateChanged;

        public GameManager()
        {
            _templates = new List<CEnemyTemplate>();
            _icons = new Dictionary<string, ImageSource>();
            _random = new Random();
            Player = new CPlayer();
        }

        public void LoadTemplates(string filePath)
        {
            string json = File.ReadAllText(filePath);
            _templates = JsonSerializer.Deserialize<List<CEnemyTemplate>>(json);
            NormalizeChances();
        }

        private void NormalizeChances()
        {
            double sum = 0;
            foreach (var t in _templates)
                sum += t.GetSpawnChance();
            if (sum == 0) return;
            foreach (var t in _templates)
                t.SetSpawnChance(t.GetSpawnChance() / sum);
        }

        public void LoadIcons(string iconsPath)
        {
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, iconsPath);
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            var files = Directory.GetFiles(fullPath, "*.png");
            foreach (var file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                var bitmap = LoadImageFromFile(file);
                if (bitmap != null)
                    _icons[name] = bitmap;
            }
        }

        private BitmapImage LoadImageFromFile(string filePath)
        {
            try
            {
                var bitmap = new BitmapImage();
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public CEnemyTemplate SelectRandomTemplate()
        {
            double chance = _random.NextDouble();
            double cumulative = 0;
            foreach (var t in _templates)
            {
                cumulative += t.GetSpawnChance();
                if (chance <= cumulative)
                    return t;
            }
            return _templates.LastOrDefault();
        }

        public void SpawnEnemy()
        {
            if (_templates.Count == 0) return;
            var template = SelectRandomTemplate();
            if (template == null) return;
            var icon = _icons.ContainsKey(template.GetIconName()) ? _icons[template.GetIconName()] : null;
            CurrentEnemy = new CEnemy(template, Player.Level, icon);
            StateChanged?.Invoke();
        }

        public void ClickEnemy()
        {
            if (CurrentEnemy == null) return;
            bool isDead = CurrentEnemy.TakeDamage(Player.Damage);
            if (isDead)
            {
                Player.AddGold(CurrentEnemy.GoldReward);
                SpawnEnemy();
            }
            StateChanged?.Invoke();
        }

        public void RepeatEnemy()
        {
            if (CurrentEnemy != null)
            {
                CurrentEnemy.Heal();
                StateChanged?.Invoke();
            }
        }

        public void NextEnemy()
        {
            SpawnEnemy();
        }

        public void Upgrade()
        {
            Player.Upgrade();
            StateChanged?.Invoke();
        }
    }
}