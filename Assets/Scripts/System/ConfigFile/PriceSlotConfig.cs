using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class PriceSlotConfigRecord
{
    [SerializeField]
    private int idSlot;
    [SerializeField]
    private int price;
    [SerializeField]
    private Currency currency;

    public int IdSlot { get { return idSlot; } set { idSlot = value; } }
    public int Price { get { return price; } set { price = value; } }
    public Currency Currency { get { return currency; } set { currency = value; } }

}
public class PriceSlotConfig : BYDataTable<PriceSlotConfigRecord>
{
    public override ConfigCompare<PriceSlotConfigRecord> DefineConfigCompare()
    {
        configCompare = new ConfigCompare<PriceSlotConfigRecord>("idSlot");
        return configCompare;
    }
}
