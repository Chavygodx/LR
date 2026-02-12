namespace ClickerGame
{
    public class CPlayer
    {
        private int _level;
        private BigNumber _gold;
        private BigNumber _damage;
        private int _damageModifier;      
        private BigNumber _upgradeCost;
        private double _upgradeModifier;

        public int Level => _level;
        public BigNumber Gold => _gold;
        public BigNumber Damage => _damage;
        public BigNumber UpgradeCost => _upgradeCost;

        public CPlayer()
        {
            _level = 1;
            _gold = new BigNumber(0);
            _damage = new BigNumber(1);
            _damageModifier = 2;          
            _upgradeCost = new BigNumber(10);
            _upgradeModifier = 0.5;
        }

        public void AddGold(BigNumber amount)
        {
            _gold = _gold.Add(amount);
        }

        public bool Upgrade()
        {
            if (_gold.CompareTo(_upgradeCost) < 0)
                return false;

            _gold = _gold.Subtract(_upgradeCost);
            _damage = _damage.Multiply(_damageModifier); 
            _level++;
            _upgradeCost = _upgradeCost.Multiply(_upgradeModifier * _level);
            return true;
        }
    }
}