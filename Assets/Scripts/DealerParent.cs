using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerParent : MonoBehaviour
{
    private const string path = "Prefabs/Dealer";
    public Vector3 spacing = new Vector3(0, 0, 0);
    [SerializeField] private List<Dealer> _dealers = new();

    private void OnEnable()
    {
        DataTrigger.RegisterValueChange(DataPath.LEVEL, OnLevelChange);
    }

    public void Init()
    {
        StartCoroutine(InitDealerCoroutine());
    }

    private void OnLevelChange(object newLevel)
    {
        int level = (int)newLevel;
        if (level % 5 == 0) NextDealerCanUnlock();
    }

    private IEnumerator InitDealerCoroutine()
    {
        //Debug.LogWarning("init dealer");

        var dealersData = DataAPIController.instance.GetAllDealerData();
        CardType type = IngameController.instance.CurrentCardType;
        var allSlotData = DataAPIController.instance.AllSlotDataInDict(type);
        int activeDealerCount = -1;

        for (int i = 0; i < dealersData.Count; i++)
        {
            Dealer dealer = Instantiate(Resources.Load<Dealer>(path), transform);
            dealer.Id = i;
            dealer.dealSlot.ID = i;
            dealer.Init();
            dealer.transform.position += spacing;
            spacing += new Vector3(2.25f, 0);
            _dealers.Add(dealer);
            dealer.dealSlot.Init();
           
            if (dealer.Status == SlotStatus.Active || dealer.Status == SlotStatus.Locked) activeDealerCount++;
            if (dealer.Status == SlotStatus.Active)
            {

                dealer.dealSlot.onToucheHandle.AddListener(dealer.dealSlot.TapHandler);

            }
            else
            {
                dealer.SetFillActive(false);
                dealer.SetUpgradeButtonActive(false);
                dealer.SetDealerLvelActive(false);
            }
            yield return null; // Spread initialization over multiple frames
        }

        if (activeDealerCount > 0)
        {
            UpdateFill(activeDealerCount + 2, 0);
        }
        else
        {
            for (int i = 0; i < _dealers.Count; i++)
            {
                var slotData = allSlotData[i];
                _dealers[i].dealSlot.LoadCardData(slotData.currentStack);
            }
        }
        gameObject.SetActive(false);
    }

    public void NextDealerCanUnlock()
    {
        //Debug.Log("Next dealer can unlock");
        var d = _dealers.FindLast(dealer => dealer.Status == SlotStatus.Active);
        if (d != null)
        {
            int nextID = d.Id + 1;
            if (nextID < _dealers.Count)
            {
                Dealer nextD = _dealers[nextID];
                nextD.transform.DOMoveX(d.transform.position.x + 3f, 0.1f);
                var nextDealerData = DataAPIController.instance.GetDealerData(nextID);
                nextDealerData.status = SlotStatus.Locked;
                nextD.Init();
                nextD.dealSlot.Init();
                nextD.SetDealerAndFillActive(false);
                nextD.SetRewardActive(false);
                nextD.SetFillActive(false);
                nextD.SetDealerLvelActive(false);
                UpdateFill(4, 0.5f, null);
                nextD.gameObject.SetActive(true);
                Debug.LogWarning($"Next dealer ID is {nextD.Id}");
            }
            else
            {
                //Debug.LogWarning("Next dealer ID is out of range.");
                return;
            }
        }
        else
        {
            return;
            //Debug.LogWarning("No active dealer found.");
        }
    }

    private void UpdateFill(int count, float time, Action callback = null)
    {
        StartCoroutine(UpdateFillCoroutine(count, time, callback));
    }
    Tween t;
    private IEnumerator UpdateFillCoroutine(int count, float time, Action callback)
    {
        float xTarget;
        int index;
        var allSlotData = DataAPIController.instance.AllSlotDataInDict(IngameController.instance.CurrentCardType);
        SlotData slotData;
        Vector3 pos;
        for (int i = 0; i < count; i++)
        {

            index = i;  // Capture the current index for the closure
            pos = _dealers[index].transform.position;
            xTarget = pos.x - 1.25f;

            t = _dealers[index].transform.DOMoveX(xTarget, time);

            t.OnUpdate(() =>
            {
                _dealers[index]._anchorPoint.DOMoveX(xTarget, time);
                _dealers[index].UpdateFillPostion();
            });
            t.OnComplete(() =>
            {
                slotData = allSlotData[index];
                _dealers[index].dealSlot.LoadCardData(slotData.currentStack);
                t.Kill();
            });

            yield return t.WaitForCompletion();
        }

        callback?.Invoke();
    }

    private void OnDestroy()
    {
        foreach (Dealer d in _dealers)
        {
            d.SetDealerAndFillActive(false);
        }
    }
    public Dealer GetDealerAtSLot(int index)
    {
        Dealer dealer = _dealers[index];
        if (dealer != null) return dealer;
        else return null;
    }
    public List<Dealer> ActiveDealers()
    {
        return _dealers.FindAll(dealer => dealer.Status == SlotStatus.Active);
    }
}
