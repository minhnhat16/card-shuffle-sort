using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShopConfigRecord {

    [SerializeField]
    private int id;
    [SerializeField]    
    private List<int> idPrice;
    public int Id { get => id; }
    public List<int> IdPrice { get => idPrice; }
}

public class ShopConfig : BYDataTable<ShopConfigRecord>
{
    public override ConfigCompare<ShopConfigRecord> DefineConfigCompare()
    {
        configCompare = new ConfigCompare<ShopConfigRecord>("id");
        return configCompare;
    }
}
