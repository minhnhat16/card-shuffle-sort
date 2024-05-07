
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DataAPIController : MonoBehaviour
{
    public static DataAPIController instance;

    [SerializeField]
    private DataModel dataModel;

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
    }
    public void InitData(Action callback)
    {
        Debug.Log("(BOOT) // INIT DATA");

        dataModel.InitData(() =>
        {
            // CheckDailyLogin();
            callback();
        });
        Debug.Log("==========> BOOT PROCESS SUCCESS <==========");
    }

    #region Get Data
    public int GetPlayerLevel()
    {
        Debug.Log("DATA === LEVEL");
        return dataModel.ReadData<int>(DataPath.LEVEL);
    }

    #region CARDTYPE DATA & CARD TYPE LIST
   
    public CardType GetCurrentCardType()
    {
        CardType cardType = dataModel.ReadData<CardType>(DataPath.CURRENTCARDTYPE);
        return cardType;
    }
    public Dictionary<string, ListCardColor> GetAllCardColor()
    {
        var listCardType = dataModel.ReadData<Dictionary<string, ListCardColor>>(DataPath.LISTCOLORBYTYPE);
        return listCardType;
    }
    public ListCardColor GetDataColorByType(CardType cardTypeKey)
    {
        ListCardColor listCardType = dataModel.ReadDictionary<ListCardColor>(DataPath.LISTCOLORBYTYPE, cardTypeKey.ToString());
        return listCardType;
    }

    public void SaveCardColorToList(string currentCard, CardColor newColor,Action callback)
    {
         ListCardColor currentColors = /*GetAllCardColor(currentCard)*/new ListCardColor();
        currentColors.color.Add(newColor);
        dataModel.UpdateDataDictionary(DataPath.LISTCOLORBYTYPE, currentCard, currentColors, () =>
        {
            Debug.Log("SaveCardColorToList: DONE");
            callback?.Invoke();
        });
    }
    #endregion
    #region CURRRENCY
    public int GetGold()
    {
        CurrencyWallet goldWallet = dataModel.ReadDictionary<CurrencyWallet>(DataPath.WALLETINVENT, Currency.Gold.ToString());
        return goldWallet.amount;
    }
    public int GetGem()
    {
        CurrencyWallet gemWallet = dataModel.ReadDictionary<CurrencyWallet>(DataPath.WALLETINVENT, Currency.Gem.ToString());
        return gemWallet.amount;
    }
    public void MinusGold(int minus, Action<bool> callback)
    {

        CurrencyWallet goldWallet = dataModel.ReadDictionary<CurrencyWallet>(DataPath.WALLETINVENT, Currency.Gold.ToString());
        goldWallet.amount -= minus;
        SaveGold(goldWallet, callback);
    }

    public void SetLevel(int playerLevel, Action callback)
    {
        int currentLevel = GetPlayerLevel();
        dataModel.UpdateData(DataPath.LEVEL, playerLevel,() =>
        {
            Debug.Log($"Save level done at {currentLevel}");
            callback?.Invoke();
        });
    }

    public void AddGold(int add)
    {
        CurrencyWallet gold = dataModel.ReadDictionary<CurrencyWallet>(DataPath.WALLETINVENT, Currency.Gold.ToString());
        gold.amount += add;
        SaveGold(gold, null);
    }
    public void SaveGold(CurrencyWallet gold, Action<bool> callback)
    {
        dataModel.UpdateDataDictionary<CurrencyWallet>(DataPath.WALLETINVENT, Currency.Gold.ToString(), gold, () =>
        {
            callback?.Invoke(true);
            return;
        });
        callback?.Invoke(false);

    }
    public void AddGem(int add)
    {
        CurrencyWallet gold = dataModel.ReadDictionary<CurrencyWallet>(DataPath.WALLETINVENT, Currency.Gem.ToString());
        gold.amount += add;
        SaveGem(gold, null);
    }
    #endregion
    #region SLOT & DEALER
    public SlotData GetSlotData(int key)
    {
        SlotData newSlotData = dataModel.ReadDictionary<SlotData>(DataPath.SLOTDICT, DataTrigger.ToKey(key));
        return newSlotData;
    }
    public DealerData GetDealerData(int key)
    {
        string stringKey = DataTrigger.ToKey(key);
        Debug.Log($"String key  { stringKey}");
        DealerData newDealerData = dataModel.ReadDictionary<DealerData>(DataPath.DEALERDICT, stringKey);
        return newDealerData;
    }
    public void SaveSlotData(int key, SlotData newSlotData, Action<bool> callback)
    {

        dataModel.UpdateDataDictionary(DataPath.SLOTDICT, DataTrigger.ToKey(key), newSlotData ,() =>
        {
            callback?.Invoke(true);
        });
    }
    #endregion

    #endregion
    #region daytimedata
    public string GetDayTimeData()
    {
        string day = dataModel.ReadData<string>(DataPath.DAYCHECKED);
        //Debug.Log($"day {day}");
        return day;
    }
    public void SetDayTimeData(string day)
    {
        if (!string.IsNullOrEmpty(day))
        {
            dataModel.UpdateData(DataPath.DAYCHECKED, day, () =>
             {
                 Debug.Log("SAVE DAYTIME DATA SUCCESSFULL");
             });
        }
    }
    #endregion
    #region Others
    public float GetCurrentExp()
    {
        return dataModel.ReadData<float>(DataPath.EXPCURRENT);
    }
    public void SetCurrentExp(float currentExp, Action callback)
    {
        dataModel.UpdateData(DataPath.EXPCURRENT, currentExp, () =>
        {
            Debug.Log($"Save current exp to data successfull {currentExp}");
            callback?.Invoke();
        });
    }
    public ItemData GetItemData(string type)
    {
        Debug.Log("DATA === ITEM DATA");
        ItemData itemData = dataModel.ReadDictionary<ItemData>(DataPath.ITEM, type);
        return itemData;
    }
    public int GetItemTotal(string type)
    {
        //Debug.Log("GetItemTotal");
        ItemData itemData = dataModel.ReadDictionary<ItemData>(DataPath.ITEM, type);
        int total = itemData.total;
        //Debug.Log($"TOTAL ITEM{itemData.id} {total}");
        return total;
    }
    public void AddItemTotal(string type, int inTotal)
    {
        Debug.Log("DATA === ADD ITEMDATA");
        inTotal += GetItemTotal(type);
        SetItemTotal(type, inTotal);
    }
    public void SetItemTotal(string type, int inTotal)
    {
        Debug.Log("DATA === SAVE ITEMDATA");
        ItemData itemData = new()
        {
            //id = type,
            total = inTotal,
        };
        dataModel.UpdateDataDictionary(DataPath.ITEM, type.ToString(), itemData);
    }

 
    public Dictionary<string, DailyData> GetAllDailyData()
    {
        var dailyData = dataModel.ReadData<Dictionary<string, DailyData>>(DataPath.DAILYDATA);
        return dailyData;
    }
    public void SetNewDailyCircle()
    {
        for (int i = 1; i <= 7; i++)
        {
            DailyData dailyData = new();
            dailyData.day = i;
            dailyData.type = IEDailyType.Unavailable;
            dataModel.UpdateDataDictionary(DataPath.DAILYDATA, i.ToString(), dailyData);
        }
    }
    public DailyData GetDailyData(string key)
    {
        DailyData dailyData = dataModel.ReadDictionary<DailyData>(DataPath.DAILYDATA, key);
        return dailyData;
    }
    public void SetDailyData(string day, IEDailyType type)
    {
        DailyData dailyData = dataModel.ReadDictionary<DailyData>(DataPath.DAILYDATA, day);
        dailyData.type = type;
        dataModel.UpdateDataDictionary(DataPath.DAILYDATA, day, dailyData);
    }
    #endregion
}
