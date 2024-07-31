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
    public List<Vector3> dealerPosition;

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
        if (level % 2 == 0)
        {
            NextDealerCanUnlock();
        }
    }

    private IEnumerator InitDealerCoroutine()
    {
        var dealersData = DataAPIController.instance.GetAllDealerData();
        CardType type = IngameController.instance.CurrentCardType;
        var allSlotData = DataAPIController.instance.AllSlotDataInDict(type);

        for (int i = 0; i < dealersData.Count; i++)
        {
            Dealer dealer = Instantiate(Resources.Load<Dealer>(path), transform);
            dealer.Id = i;
            dealer.dealSlot.ID = i;
            dealer.Init();
            _dealers.Add(dealer);
            dealer.dealSlot.gameObject.SetActive(true);
            dealer.dealSlot.Init();

            if (dealer.Status == SlotStatus.Active || dealer.Status == SlotStatus.Locked)
            {
                if (dealer.Status == SlotStatus.Active)
                {
                    dealer.dealSlot.onToucheHandle.AddListener(dealer.dealSlot.TapHandler);
                    dealer.dealSlot.BoxCol.isTrigger = false;
                }
                else
                {
                    dealer.SetFillActive(false);
                    dealer.SetUpgradeButtonActive(false);
                    dealer.SetDealerLvelActive(false);
                    dealer.SetRewardActive(false);
                }
            }
            else
            {
                dealer.SetFillActive(false);
                dealer.SetUpgradeButtonActive(false);
                dealer.SetDealerLvelActive(false);
                dealer.SetRewardActive(false);
            }
        }

        yield return new WaitForSeconds(1f);
        int c = TotalActiveDealer();
        if (c > 1)
        {
            NewUpdateDealerPosition(c);
        }
        else
        {
            // For a single dealer, position at x = 0
            NewUpdateFillWithId(0, 0);
        }
    }

    public void NewUpdateDealerPosition(int count)
    {
        Debug.LogWarning("TotalActiveDealer: " + count);

        switch (count)
        {
            case 1:
                NewUpdateFillWithId(0, 0); // Center position for single dealer
                break;

            case 2:
                NewUpdateFillWithId(0, -1.125f); // Left
                NewUpdateFillWithId(1, 1.125f);  // Right
                break;

            case 3:
                NewUpdateFillWithId(0, -2.25f);  // Left
                NewUpdateFillWithId(1, 0);       // Center
                NewUpdateFillWithId(2, 2.25f);   // Right
                break;

            case 4:
                NewUpdateFillWithId(0, -3.375f); // Far Left
                NewUpdateFillWithId(1, -1.125f); // Left
                NewUpdateFillWithId(2, 1.125f);  // Right
                NewUpdateFillWithId(3, 3.375f);  // Far Right
                break;

            default:
                Debug.LogWarning("Unsupported number of dealers: " + count);
                break;
        }
    }

    public void NewUpdateFillWithId(int index, float xTarget)
    {
        Debug.LogWarning("NewUpdateFillWithId " + index);
        var d = _dealers[index];
        Tween t = d.transform.DOMoveX(xTarget, 0.5f);
        t.OnUpdate(() =>
        {
            d._anchorPoint.DOMoveX(xTarget, 0.5f);
            d.UpdateFillPostion();

            if (d.Status == SlotStatus.Active) d.dealSlot.UpdateCardPositionX(d.transform.position);
        });
        t.OnComplete(() =>
        {
            if (d.Status == SlotStatus.InActive)
            {
                d.SetDealerLvelActive(false);
                d.SetFillActive(false);
                d.SetUpgradeButtonActive(false);
                d.SetDealerLvelActive(false);
                d.SetRewardActive(false);
            }
            else if (d.Status == SlotStatus.Locked)
            {
                d.SetFillActive(false);
                d.SetUpgradeButtonActive(false);
                d.SetDealerLvelActive(false);
                d.SetRewardActive(false);
                d.dealSlot.SettingBuyBtn(true);
                d.goldGroup.SetPositionWithParent(d.gameObject);
                d.gemGroup.SetPositionWithParent(d.gameObject);
                d.gameObject.SetActive(true);
            }
            else
            {

                var dealerData = DataAPIController.instance.GetDealerData(d.Id);
                d.dealSlot.LoadCardData(dealerData.currentStack);
            }
            t.Kill();
        });
    }

    public void NextDealerCanUnlock()
    {
        Dealer canUnlockDealer = FindInActiveDealer();
        if (canUnlockDealer != null)
        {
            NewUpdateFillWithId(canUnlockDealer.Id, canUnlockDealer.transform.position.x);
            canUnlockDealer.Status = SlotStatus.Locked;
            var nextDealerData = DataAPIController.instance.GetDealerData(canUnlockDealer.Id);
            nextDealerData.status = SlotStatus.Locked;
            switch (canUnlockDealer.Id)
            {
                case 0: break;
                case 1:
                    NewUpdateFillWithId(0, -1.125f);
                    NewUpdateFillWithId(1, 1.125f);

                    break;
                case 2:
                    NewUpdateFillWithId(0, -3.375f);
                    NewUpdateFillWithId(1, -1.125f);
                    NewUpdateFillWithId(2, 1.125f);
                    break;
                case 3:
                    NewUpdateFillWithId(0, -3.375f);
                    NewUpdateFillWithId(1, -1.125f);
                    NewUpdateFillWithId(2, 1.125f);
                    NewUpdateFillWithId(3, 3.125f);

                    break;
            }
        }
    }

    public Dealer FindInActiveDealer()
    {
        foreach (var d in _dealers)
        {
            if (d.Status == SlotStatus.InActive) return d;
        }
        return null;
    }

    public void SaveDataDealer(CardType type)
    {
        foreach (Dealer dealer in _dealers)
        {
            dealer.dealSlot.SaveCardListToData(type);
            Debug.Log("Save Data Dealer");
            dealer.SetDealerAndFillActive(false);
            dealer.SetDealerLvelActive(false);
            dealer.SetFillActive(false);
        }
    }

    public void SetupOnDestroy()
    {
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        foreach (Dealer dealer in _dealers)
        {
            dealer.dealSlot.BuyBtn.gameObject.SetActive(false);
            dealer.SetDealerAndFillActive(false);
            dealer.SetDealerLvelActive(false);
            dealer.SetFillActive(false);
            dealer.dealSlot.SaveCardListToData(IngameController.instance.CurrentCardType);
            yield return null;
        }
        Debug.Log("Done Save Data");
    }

    public Dealer GetDealerAtSlot(int index)
    {
        return _dealers[index];
    }

    public List<Dealer> ActiveDealers()
    {
        return _dealers.FindAll(dealer => dealer.Status == SlotStatus.Active);
    }

    public int TotalActiveDealer()
    {
        return _dealers.FindAll(dealer => dealer.Status == SlotStatus.Active || dealer.Status == SlotStatus.Locked).Count;
    }
}
