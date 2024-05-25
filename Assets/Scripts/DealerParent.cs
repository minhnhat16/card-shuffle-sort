using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DealerParent : MonoBehaviour
{
    protected private const string path = "Prefabs/Dealer";
    public Vector3 spacing = new Vector3(0, 0, 0);
    [SerializeField] private List<Dealer> _dealers = new();
    public List<Dealer> Dealers { get { return _dealers; } set { _dealers = value; } }
    private List<SlotData> allSlotData = new();
    public Dealer GetDealerByID(int id)
    {
        return id >= 0 && id < _dealers.Count ? _dealers[id] : null;
    }

    public void SetDealerByID(int id, Dealer dealer)
    {
        if (dealer == null || id < 0 || id >= _dealers.Count) return;
        _dealers[id] = dealer;
    }

    private void OnEnable()
    {
        DataTrigger.RegisterValueChange(DataPath.LEVEL, (newLevel) =>
        {
            Debug.Log("NEW LEVEL UNLOCK");
            int level = (int)newLevel;
            if (level % 5 == 0) NextDealerCanUnlock();
        });
    }
    private void Start()
    {
        allSlotData = new();
    }
    public void Init()
    {
        InitDealer();
    }

    public void InitDealer()
    {
        var dealersData = DataAPIController.instance.GetAllDealerData();
        CardType type = IngameController.instance.CurrentCardType;
        allSlotData = DataAPIController.instance.AllSlotDataInDict(type);
        int activeDealerCount = -1;
        for (int i = 0; i < dealersData.Count; i++)
        {
            //var slotRecord = all[i];
            //SlotData slotData = DataAPIController.instance.GetSlotDataInDict(i, currentCardType);
            var slotData = allSlotData[i];
            Dealer dealer = Instantiate(Resources.Load<Dealer>(path), transform);
            dealer.Id = i;
            dealer.dealSlot.ID = i;
            dealer.Init();
            dealer.dealSlot.Init();
            dealer.transform.position += spacing;
            spacing += new Vector3(2.25f, 0);
            _dealers.Add(dealer);
            dealer.dealSlot.Init();
            if (dealer.Status == SlotStatus.Active || dealer.Status == SlotStatus.Locked) activeDealerCount++;
            dealer.UpdateFillPostion();
        }
        int time = 0;
        Debug.LogWarning($"Active dealer count >0" + activeDealerCount);

        if (activeDealerCount > 0)
        {
            UpdateFill(activeDealerCount + 1, time, () =>
            {
                Debug.LogWarning($"Active dealer count >0");
                int i = 0;
                foreach (var dealer in _dealers)
                {
                    
                    i++;
                }
            });
        }
        else
        {
            int i = 0;
            Debug.LogWarning($"Active dealer count <0");

            foreach (var dealer in _dealers)
            {
                var slotData = allSlotData[i];
                dealer.dealSlot.LoadCardData(slotData.currentStack);
                i++;
            }
        }

    }
    public void DoMoveParentToLeft(int count)
    {
        //UpdateFill(count, 1f);
    }
    public void NextDealerCanUnlock()
    {
        Debug.Log("Next dealer can unlock");
        var d = _dealers.FindLast(dealer => dealer.Status == SlotStatus.Active);
        if (d != null)
        {
            int nextID = d.Id + 1;
            if (nextID < _dealers.Count)
            {
                Dealer nextD = _dealers[nextID];
                var nextDealerData = DataAPIController.instance.GetDealerData(nextID);
                nextDealerData.status = SlotStatus.Locked;
                nextD.Init();
            }
            else
            {
                Debug.LogWarning("Next dealer ID is out of range.");
            }
        }
        else
        {
            Debug.LogWarning("No active dealer found.");
        }

    }

    Tween t;
    private void UpdateFill(int count, float time, Action callback)
    {
        Debug.Log("Update fill" + count);
        for (int i = 0; i < count; i++)
        {
            int index = i;  // Capture the current index for the closure
            Vector3 pos = _dealers[index].transform.position;
            float xTarget = pos.x - 1.125f;
            t = _dealers[index].transform.DOMoveX(xTarget, time).OnUpdate(() =>
             {
                 _dealers[index]._anchorPoint.DOMoveX(xTarget, time);
                 _dealers[index].UpdateFillPostion();
             });
            t.OnComplete(() =>
            {
                var slotData = allSlotData[i];
                Debug.LogWarning($"Active dealer count >0 load card data");
                _dealers[index].dealSlot.LoadCardData(slotData.currentStack);
                t.Kill();
            });
        }

    }
}
