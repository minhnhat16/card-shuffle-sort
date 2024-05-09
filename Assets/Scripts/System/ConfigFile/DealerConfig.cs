using UnityEngine;


[System.Serializable]
public class DealerConfigRecord
{
    [SerializeField]
    private int levelId;
    [SerializeField]
    private Currency currencyType;
    [SerializeField]
    private int cost;
    [SerializeField]
    private int levelGold;
    [SerializeField]
    private int levelGem;
    public int LevelId { get => levelId; set => levelId = value; }
    public Currency CurrencyType { get => currencyType; set => currencyType = value; }
    public int Cost { get => cost; set => cost = value; }
    public int LevelGold { get => levelGold; set => levelGold = value; }
    public int LevelGem { get => levelGem; set => levelGem = value; }
}
public class DealerConfig : BYDataTable<DealerConfigRecord>
{
    public override ConfigCompare<DealerConfigRecord> DefineConfigCompare()
    {
        configCompare = new ConfigCompare<DealerConfigRecord>("levelId");
        return configCompare;
    }
}
