using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData
{
    [SerializeField]
    public UserInfo userInfo;
    [SerializeField]
    public LevelInfo levelInfo;
    [SerializeField]
    public ItemInvent itemInventory;
    [SerializeField]
    public CardInventory cardInvent;
    [SerializeField]
    public Wallet wallet;
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
public class LevelInfo
{
    public int level;
    public float expLevel;
}
[Serializable]
public class ItemInvent
{
    public Dictionary<ItemType, ItemData> itemDict;
}
[Serializable]
public class DailyData
{
    public int day;
    public IEDailyType type;
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
    public CardType currentCardType;
    public CardType type;
    public Dictionary<CardType, ListCardColor> listColorByType;
}
[Serializable]
public class ListCardColor
{
    public List<CardColor> color;
}
[Serializable]
public class Wallet
{
    public Dictionary<Currency, CurrencyWallet> walletInvent;
}
[Serializable]
public class CurrencyWallet
{
    public Currency id;
    public int amount;
}


