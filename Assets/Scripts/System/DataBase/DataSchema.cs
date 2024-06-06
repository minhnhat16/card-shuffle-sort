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
    public List<DailyItemData> dailyData;
    [SerializeField]
    public SlotDataDict allSlotData;
    [SerializeField]
    public Dictionary<string, DealerData> dealerDict;
    [SerializeField]
    public SlotCameraData cameraData;
    [SerializeField]
    public CardCounter cardCounter;
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
    public ItemData bombItem;
    public ItemData magnetItem;
}
[Serializable]
public class DailyItemData
{
    public int day;
    public IEDailyType currentType;
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
    public Dictionary<string, ListCardColor> listColorByType;
}
[Serializable]
public class ListCardColor
{
    public List<CardColorPallet> color;
}
[Serializable]
public class Wallet
{
    public CurrencyWallet goldWallet;
    public CurrencyWallet gemWallet;
}
[Serializable]
public class CurrencyWallet
{
    public Currency currency;
    public int amount;
}
[Serializable]
public class SlotDataDict
{
    public Dictionary<string,List<SlotData>> slotDict;
}
public class SlotData
{
    public int id;
    public SlotStatus status;
    public Stack<CardColorPallet> currentStack;
}
[Serializable]

public class DealerData
{
    public int id;
    public SlotStatus status;
    public int upgradeLevel;
    public Stack<CardColorPallet> currentStack;
}
[Serializable]

public class SlotCameraData
{
    public int scaleTime;
    public float  positionX;
    public float positionY;
    public float OrthographicSize;
}
[Serializable]
public class CardCounter
{
    public string lastSaveTime;
    public string currentTime;
    public int currentCardPool;
    public int maxCardPool;
}