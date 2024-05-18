using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class IngameController : MonoBehaviour
{
    public static IngameController instance;
    [SerializeField] List<List<Slot>> slots = new List<List<Slot>>();
    List<Slot> _slot = new();
    [SerializeField] List<Slot> _slotSorted = new();
    [SerializeField] private int playerLevel;
    [SerializeField] private float exp_Current;
    [SerializeField] private CardType _currentCardType;
    [SerializeField] public DealerParent dealerParent;
    [HideInInspector] public UnityEvent<int> onGoldChanged;
    [HideInInspector] public UnityEvent<int> onCurrencyChanged;
    [HideInInspector] public UnityEvent<int> onDealerClaimGold;
    [HideInInspector] public UnityEvent<int> onDealerClaimGem;
    [HideInInspector] public UnityEvent<float> onExpChange;
    public float Exp_Current { get { return exp_Current; } set { exp_Current = value; } }
    public int GetPlayerLevel()
    {
        Debug.Log($"Player level {playerLevel}");
        return playerLevel;
    }

    public void SetPlayerLevel(int level)
    {
        if (level <= playerLevel) return;
        Debug.Log($"Player level up to {level}");
        playerLevel = level;
        // note: Set data to player through DataApiController
        DataAPIController.instance.SetLevel(level, () =>
        {
            Debug.Log($"Save level up to data {level}");

        });
    }
    private void OnEnable()
    {
        DataTrigger.RegisterValueChange(DataPath.SLOTDICT, (key) =>
        {
            string stringKey = key.ToString();
            //Debug.Log("String key" + stringKey);
            //ChangeAfterUnlockSlot(stringKey);
        });
    }
    private void OnDisable()
    {
        DataTrigger.UnRegisterValueChange(DataPath.SLOTDICT, (key) =>
        {
        });
    }
    private void Awake()
    {
        instance = this;
    }
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        InitCardSlot(() =>
        {
            ViewManager.Instance.SwitchView(ViewIndex.GamePlayView);
        });
        playerLevel = DataAPIController.instance.GetPlayerLevel();
        exp_Current = DataAPIController.instance.GetCurrentExp();
        _currentCardType = DataAPIController.instance.GetCurrentCardType();
        GameManager.instance.GetCardListColorFormData(_currentCardType);
        dealerParent.Init();
    }
    protected internal void InitCardSlot(Action callback)
    {
        var all = ConfigFileManager.Instance.SlotConfig.GetAllRecord();
        var _PriceConfig = ConfigFileManager.Instance.PriceSlotConfig;
        int row = 0;
        for (int i = 0; i < all.Count; i++)
        {
            var slotRecord = all[i];
            Slot newSlot = SlotPool.Instance.pool.SpawnNonGravity();
            PriceSlotConfigRecord slotConfigRecord = _PriceConfig.GetRecordByKeySearch(i);
            SlotData data = DataAPIController.instance.GetSlotData(i) ?? null;
            newSlot.ID = slotRecord.ID;
            newSlot.FibIndex = slotRecord.FibIndex;
            newSlot.transform.position = slotRecord.Pos;

            if (data != null && data.isUnlocked) newSlot.status = SlotStatus.Active;
            else newSlot.status = all[i].Status;
            newSlot.SetSprite();
            if (slotConfigRecord != null)
            {
                Debug.Log("(SLOT) SLOT HAVE PRICE SLOT CONFIG");
                int idSlot = slotConfigRecord.IdSlot;
                int price = slotConfigRecord.Price;
                Currency type = slotConfigRecord.Currency;
                newSlot.SetSlotPrice(idSlot, price, type);
            }
            newSlot.EnableWhenInCamera();

            //slots[row][i] = newSlot;
            if (i % 7 == 0) row++;
            _slot.Add(newSlot);
            if (i == all.Count - 1) {
                //SettingInactiveSlot();
                callback?.Invoke();
            }
            ;
        }
        _slotSorted = _slot;
        _slotSorted.Sort((slot1, slot2) => slot1.FibIndex.CompareTo(slot2.FibIndex));
    }

    public List<Slot> GetNeighbors(Slot slot)
    {
        var neighbors = new List<Slot>();

        foreach (var s in _slot)
        {
            if (s.CheckNeighborSlot(slot))
            {
                neighbors.Add(s);
            }
        }
        return neighbors;
    }
    public void SwitchNearbyCanUnlock(Slot slot)
    {
        Debug.Log("SET NEARBY CAN UNLOCK" + slot.ID);
        var neighbors = GetNeighbors(slot);
        Debug.Log($"NEIGHBOR COUNT {neighbors.Count}");
        foreach (var neighbor in neighbors)
        {

            UpdateNearbyNeigbor(neighbor);
        }
    }

    public void SwitchNearbyInActive(Slot slot)
    {
        var neighbors = GetNeighbors(slot);
        foreach (var neighbor in neighbors)
        {
            neighbor.gameObject.SetActive(slot.status != SlotStatus.InActive);
        }
    }
    public void UpdateNearbyNeigbor(Slot nei)
    {
        if (nei.status == SlotStatus.Active) return;
        int ID = nei.ID;
        if(nei.gameObject.activeSelf)  nei.gameObject.SetActive(true);
        if (nei.status == SlotStatus.InActive || nei.status == SlotStatus.Locked)
        {
            if (nei.gameObject.activeSelf == false) nei.gameObject.SetActive(true);
            var priceSlotConfig = ConfigFileManager.Instance.PriceSlotConfig.GetRecordByKeySearch(ID);
            nei.status = SlotStatus.Locked;
            nei.SetSlotPrice(ID, priceSlotConfig.Price, priceSlotConfig.Currency);
            nei.UpdateSlotConfig();
            nei.EnableWhenInCamera();
            nei.SetSprite();
            nei.SettingBuyBtn(true);
        }
       
    }
    
    public List<Slot> GetListSlotInActive()
    {
        return SlotPool.Instance.pool.activeList.Where(slot => slot.status == SlotStatus.InActive).ToList();
    }
    public List<Slot> GetListSlotActive()
    {
        return SlotPool.Instance.pool.activeList.Where(slot => slot.status == SlotStatus.Active).ToList();
    }
    public void AllSlotCheckCamera()
    {
        for (int i = 0; i < _slot.Count; i++)
        {
            Debug.Log($"Slot {i} enable incamera");
            _slot[i].EnableWhenInCamera();
        }
    }
}
