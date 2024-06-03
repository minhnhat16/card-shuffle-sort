using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DailyGrid : MonoBehaviour
{
    public Dictionary<int, DailyItem> DailyItems;
    [SerializeField] private List<DailyItem> _items;
    [SerializeField] private GridLayoutGroup _content;
    public DailyItem currentDaily;
    public bool isNewDay;
    [HideInInspector] public UnityEvent<bool> newDateEvent = new UnityEvent<bool>();
    [HideInInspector] public UnityEvent<bool> resetDailyEvent = new UnityEvent<bool>();
    [HideInInspector] public UnityEvent<bool> lastItemClaimEvent = new UnityEvent<bool>();
    private void OnEnable()
    {
        newDateEvent.AddListener(NewDayRewardRemain);
        DataTrigger.RegisterValueChange(DataPath.LASTSAVETIME, (data) =>
        {
            if (data == null) return;
            if (data == null) return;
            string newData = data as string;
        });
    }
    private void OnDisable()
    {
        newDateEvent.RemoveListener(NewDayRewardRemain);
    }
    // Start is called before the first frame update
    void Start()
    {
        //DayTimeController.instance.CheckNewDay();
        isNewDay = DayTimeController.instance.isNewDay;
        DailyItems = new Dictionary<int, DailyItem>();
        SetupGrid();
        //CheckFullDailyClaim();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void SetupGrid()
    {
        if (DailyItems.Count != 0)
        {
            Debug.Log("SETUP GRID " + DailyItems.Count);
            return;
        }
        var dailyConfig = ConfigFileManager.Instance.DailyRewardConfig.GetAllRecord();

        for (int i = 0; i < 7; i++)
        {
            var itemDailyConfig = dailyConfig[i];
            var dailyItem = _items[i];
            DailyItems.Add(i, dailyItem);
            SetupDailyRewardItem(dailyItem, itemDailyConfig);
            _items.Add(dailyItem);
            //if (i < 6)
            //{
            //    if (dailyItem == null)
            //    {
            //        Debug.LogError(" item == null");
            //    }
            //    else
            //    {
                  
            //    }
            //}
            //else
            //{
            //    if (dailyItem == null)
            //    {
            //        Debug.LogError(" item == null");
            //    }
            //    else
            //    {
            //        DailyItems.Add(i, dailyItem.GetComponent<DailyItem>());
            //        SetupDailyRewardItem(dailyItem, itemDailyConfig);
            //        _items.Add(dailyItem);
            //    }
            //}
        }
    }
    public void InvokeWhenHaveCurrentDaily()
    {
        if (currentDaily != null && currentDaily.currentType == IEDailyType.Available) currentDaily.CheckItemAvailable();
        else
        {
            var item = _items.First((DailyItem daily) => daily.currentType == IEDailyType.Unavailable);
            item.CheckItemAvailable();
        }
    }
    public void UpdateDailyReward(DailyItem item)
    {
        if (item == null)
        {
            Debug.Log(" Daily item == null");
        }
        int index = item.day + 1;
        DailyItems[index].SwitchItemType(item.itemName);
    }
    private void CheckFullDailyClaim()
    {
        //check daily in day seven is claimed;
        var lastDailyData = DataAPIController.instance.GetDailyData(6);
        if (lastDailyData.currentType == IEDailyType.Claimed)
        {
            DataAPIController.instance.SetNewDailyCircle();
        }
    }
    private void SetupDailyRewardItem(DailyItem dailyItem, DailyRewardConfigRecord dailyRewardConfig)
    {
        if (dailyRewardConfig == null) return;
        int day = dailyRewardConfig.ID;
        Debug.Log("Day" + day);
        DailyItemData dailyData = DataAPIController.instance.GetDailyData(day);
        if (dailyData.currentType == IEDailyType.Available)
        {
            Debug.Log("AVAILABLE TO CLAIM");
            dailyItem.Init(IEDailyType.Available, dailyRewardConfig.TotalItem, dailyRewardConfig.ID + 1, dailyRewardConfig.SpriteName, dailyRewardConfig.ItemName);
            currentDaily = dailyItem;
        }
        else
        {
            Debug.Log("ELSE UNAVAILABLE TO CLAIM " + dailyData.currentType);
            dailyItem.Init(dailyData.currentType, dailyRewardConfig.TotalItem, dailyRewardConfig.ID + 1, dailyRewardConfig.SpriteName, dailyRewardConfig.ItemName);
        }
    }
    bool Predicate(KeyValuePair<int, DailyItem> kvp)
    {
        // Define your custom search criteria here
        return kvp.Value.currentType == IEDailyType.Unavailable;
    }
    public DailyItem NewDayItemAvailable()
    {
        KeyValuePair<int, DailyItem> foundItem = DailyItems.FirstOrDefault(Predicate);
        if (!foundItem.Equals(default(KeyValuePair<int, DailyItem>)))
        {
            int key = foundItem.Key;
            DailyItem item = foundItem.Value;
            // Do something with 'key' and 'item'
            return item;
        }
        else
        {
            // Item not found based on the predicate
            Debug.Log("Item not found based on the search criteria.");
            return null;
        }
    }
    public void NewDayRewardRemain(bool isNewDay)
    {
        Debug.Log("NEW DAY REWARD REMAIN" + isNewDay);
        if (isNewDay)
        {
            this.isNewDay = false;
            Debug.Log("NEW DAY REWARD REMAIN");
            var newDayItem = NewDayItemAvailable();
            Debug.Log($"new day item id {newDayItem.day}");
            newDayItem.SwitchType(IEDailyType.Available);
            DataAPIController.instance.SetDailyData(newDayItem.day--, newDayItem.currentType);
            currentDaily = newDayItem;
            currentDaily.CheckItemAvailable();
        }
        else
        {
        }
    }
}
