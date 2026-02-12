using System.Windows.Media;

namespace ClickerGame
{
    public class CEnemy
    {
        private string _name;
        private BigNumber _hitPoints;
        private BigNumber _maxHitPoints;
        private BigNumber _goldReward;
        private ImageSource _icon;

        public string Name => _name;
        public BigNumber HitPoints => _hitPoints;
        public BigNumber MaxHitPoints => _maxHitPoints;
        public BigNumber GoldReward => _goldReward;
        public ImageSource Icon => _icon;

        public CEnemy(CEnemyTemplate template, int playerLevel, ImageSource icon)
        {
            _name = template.GetName();
            _icon = icon;

         
            double lifeFactor = 1 + template.GetLifeModifier() * (playerLevel - 1);
            double goldFactor = 1 + template.GetGoldModifier() * (playerLevel - 1);

            _maxHitPoints = new BigNumber(template.GetBaseLife()).Multiply(lifeFactor);
            _hitPoints = new BigNumber(_maxHitPoints);
            _goldReward = new BigNumber(template.GetBaseGold()).Multiply(goldFactor);
        }

        public bool TakeDamage(BigNumber damage)
        {
            _hitPoints = _hitPoints.Subtract(damage);
            if (_hitPoints.CompareTo(new BigNumber(0)) < 0)
                _hitPoints = new BigNumber(0);
            return _hitPoints.CompareTo(new BigNumber(0)) == 0;
        }

        public void Heal()
        {
            _hitPoints = new BigNumber(_maxHitPoints);
        }
    }
}