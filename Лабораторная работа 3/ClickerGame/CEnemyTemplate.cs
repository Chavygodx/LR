using System.Text.Json.Serialization;

namespace ClickerGame
{
    public class CEnemyTemplate
    {
        [JsonInclude]
        private string name;

        [JsonInclude]
        private string iconName;

        [JsonInclude]
        private int baseLife;

        [JsonInclude]
        private double lifeModifier;

        [JsonInclude]
        private int baseGold;

        [JsonInclude]
        private double goldModifier;

        [JsonInclude]
        private double spawnChance;

        public CEnemyTemplate() { }

        public CEnemyTemplate(string name, string iconName, int baseLife, double lifeModifier,
                              int baseGold, double goldModifier, double spawnChance)
        {
            this.name = name;
            this.iconName = iconName;
            this.baseLife = baseLife;
            this.lifeModifier = lifeModifier;
            this.baseGold = baseGold;
            this.goldModifier = goldModifier;
            this.spawnChance = spawnChance;
        }

        [JsonIgnore]
        public string Name => name;

        public string GetName() => name;
        public string GetIconName() => iconName;
        public int GetBaseLife() => baseLife;
        public double GetLifeModifier() => lifeModifier;
        public int GetBaseGold() => baseGold;
        public double GetGoldModifier() => goldModifier;
        public double GetSpawnChance() => spawnChance;

        public void SetSpawnChance(double value) => spawnChance = value;
        public void SetName(string value) => name = value;
        public void SetIconName(string value) => iconName = value;
        public void SetBaseLife(int value) => baseLife = value;
        public void SetLifeModifier(double value) => lifeModifier = value;
        public void SetBaseGold(int value) => baseGold = value;
        public void SetGoldModifier(double value) => goldModifier = value;
    }
}