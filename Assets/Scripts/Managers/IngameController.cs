using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IngameController : MonoBehaviour
{
    public static IngameController instance;
    List<Slot> _slot = new();
    [SerializeField] private  int playerLevel;
    [SerializeField] private float exp_Current;
    [SerializeField] private CardType _currentCardType;
    [SerializeField] public DealerParent dealerParent;
    [HideInInspector] public UnityEvent<int> onGoldChanged;
    [HideInInspector] public UnityEvent<int> onCurrencyChanged;
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

        }) ;
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
        for (int i = 0; i < all.Count; i++)
        {
            Slot newSlot = SlotPool.Instance.pool.SpawnNonGravity();
            PriceSlotConfigRecord slotConfigRecord = _PriceConfig.GetRecordByKeySearch(i) == null ? null : _PriceConfig.GetRecordByKeySearch(i);
            newSlot.ID = i;
            newSlot.transform.position = all[i].Pos;
            newSlot.status = all[i].Status;
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
            if (i == all.Count - 1) callback?.Invoke();
        }
    }
  
    public List<Slot> GetListSlotActive()
    {
        if (SlotPool.Instance.pool.activeList.Count > 0)
        {
            _slot.Clear();
            for (int i = 0; i < SlotPool.Instance.pool.activeList.Count; i++)
            {
                Slot iSlot = SlotPool.Instance.pool.activeList[i];
                if (iSlot.status == SlotStatus.Active)
                {
                    _slot.Add(iSlot);
                }
             }
            return _slot;
        }

        return null;
    }
    public void SetSlotPositionByHand()
    {

    }
}
