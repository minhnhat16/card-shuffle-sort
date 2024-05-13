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

        }) ;
    }
    private void OnEnable()
    {
        DataTrigger.RegisterValueChange(DataPath.SLOTDICT, (key) =>
        {
            int id = (int)key;
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
        
        for (int i = 0; i < all.Count; i++)
        {
            Slot newSlot = SlotPool.Instance.pool.SpawnNonGravity();
            PriceSlotConfigRecord slotConfigRecord = _PriceConfig.GetRecordByKeySearch(i) ?? null;
            SlotData data = DataAPIController.instance.GetSlotData(i) ?? null;
            newSlot.ID = i;
            newSlot.transform.position = all[i].Pos;
            
            if (data != null) newSlot.status = SlotStatus.Active;
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
            _slot.Add(newSlot);
            if (i == all.Count - 1) callback?.Invoke();
        }
    }
    public void CheckTwoSlotInRow(Slot slot)
    {
        slot.transform.GetPositionAndRotation(out Vector3 slotTransform, out Quaternion slotRotation);
        List<Slot> inactiveSlot = GetListSlotInActive();
        List<Slot> canActive = new();
        for (int i = 0; i < inactiveSlot.Count; i++)
        {
            Vector3 inactivePos = inactiveSlot[i].transform.position;
            if(inactivePos.y == slotTransform.y )
            {
                if( inactivePos.x == slotTransform.x + 2 || inactivePos.x == slotTransform.x - 2)
                {
                    inactiveSlot[i].status = SlotStatus.Locked;
                    canActive.Add(inactiveSlot[i]);
                }
            }
        }
        //return canActive
    }
    public List<Slot> GetListSlotInActive()
    {
        if (SlotPool.Instance.pool.activeList.Count > 0)
        {
            List<Slot> inactiveList = new List<Slot>();
            for (int i = 0; i < SlotPool.Instance.pool.activeList.Count; i++)
            {
                Slot iSlot = SlotPool.Instance.pool.activeList[i];
                if (iSlot.status == SlotStatus.InActive)
                {
                    inactiveList.Add(iSlot);
                }
            }
            return inactiveList;
        }

        return null;
    }
    public List<Slot> GetListSlotActive()
    {
        if (SlotPool.Instance.pool.activeList.Count > 0)
        {
            List<Slot> activeList = new List<Slot>();
            for (int i = 0; i < SlotPool.Instance.pool.activeList.Count; i++)
            {
                Slot iSlot = SlotPool.Instance.pool.activeList[i];
                if (iSlot.status == SlotStatus.Active)
                {
                    activeList.Add(iSlot);
                }
            }
            return activeList;
        }

        return null;
    }
    public void AllSlotCheckCamera()
    {
        for(int i = 0; i < _slot.Count; i ++)
        {
            _slot[i].EnableWhenInCamera();
        }
    }
}
