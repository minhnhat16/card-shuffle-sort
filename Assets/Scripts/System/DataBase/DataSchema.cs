using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData
{
    [SerializeField]
    public UserInfo userInfo;
    [SerializeField]
    public Dictionary<ItemType, ItemData> itemDict;
    [SerializeField]
    public CardInventory cardInvent;
    [SerializeField]
    public Dictionary<Currency, CurrencyWallet> wallet;
    [SerializeField]
    public DailyData dailyData;
}
[Serializable]
public class UserInfo
{
    public int ID;
    public string name;
}
[Serializable]
public class DailyData
{
    public int day;
    public IEDailyType type;
}
[Serializable]
public class ItemDict
{
    
}
[Serializable]
public class ItemData
{
    public ItemType type;
    public int total;
}
[Serializable]
public class CardInventory
{
    public CardType type;
    public Dictionary<CardType, CardData> _cards;
}
[Serializable]
public class CardData
{
    public List<CardColor> color;
}

[Serializable]
public class CurrencyWallet
{
    public Currency id;
    public int amount;
}


