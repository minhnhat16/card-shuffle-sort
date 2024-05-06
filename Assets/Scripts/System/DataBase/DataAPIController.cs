
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
    public int GetGold()
    {
        //Debug.LogWarning("GETTING GOLD .....");
        int gold = dataModel.ReadData<int>(DataPath.GOLD);
        //int gold = 0;
        return gold;
    }
    public CardType GetCurrentCardType()
    {
        CardType cardType = dataModel.ReadData<CardType>(DataPath.CURRENTCARDTYPE);
        return cardType;
    }
    public ListCardColor GetAllCardColor(string cardTypeKey) 
    {
        var listCardType = dataModel.ReadData<Dictionary<CardType,ListCardColor>>(DataPath.LISTCOLORBYTYPE);
        var key = (CardType)Enum.Parse(typeof(CardType), cardTypeKey);
        return listCardType.GetValueOrDefault(key);
    }
    public ListCardColor GetAllCardColor1(string cardTypeKey)
    {
        ListCardColor listCardType = dataModel.ReadDictionary<ListCardColor>(DataPath.CURRENTCARDTYPE, cardTypeKey);
        return listCardType;
    }
    public void SaveCardColorToList(string currentCard, CardColor newColor,Action callback)
    {
         ListCardColor currentColors = GetAllCardColor(currentCard);
        currentColors.color.Add(newColor);
        dataModel.UpdateDataDictionary(DataPath.LISTCOLORBYTYPE, currentCard, currentColors, () =>
        {
            Debug.Log("SaveCardColorToList: DONE");
            callback?.Invoke();
        });
    }
    #endregion
    public void MinusGold(int minus)
    {
        int gold = dataModel.ReadData<int>(DataPath.GOLD);
        gold -= minus;
        SaveGold(gold, null);
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
        int gold = dataModel.ReadData<int>(DataPath.GOLD);
        gold += add;
        SaveGold(gold, null);
    }

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

    //public List<int> GetAllFruitSkinOwned()
    //{
    //    List<int> ownedSkins = dataModel.ReadData<List<int>>(DataPath.FRUITSKIN);
    //    return ownedSkins;
    //}
    //public void SaveFruitSkin(int id)
    //{
    //    var all = GetAllFruitSkinOwned();
    //    all.Add(id);
    //    dataModel.UpdateData(DataPath.FRUITSKIN, all);
    //}
    //public void GetAllFruitSkin()
    //{
    //    dataModel.ReadData<FruitSkin>(DataPath.FRUITSKIN);
    //}
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

    public void SaveGold(int gold, Action callback)
    {
        dataModel.UpdateData(DataPath.GOLD, gold, () =>
         {
             callback?.Invoke();
         });
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
