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
        if (level % 2 == 0)
        {
            StartCoroutine(NextDealerCanUnlock());
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
            dealer.transform.position += spacing;
            spacing += new Vector3(2.25f, 0);
            _dealers.Add(dealer);
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
            yield return null; // Spread initialization over multiple frames
        }

        if (!GameManager.instance.IsNewPlayer)
        {
            NewUpdateDealerPosition();
            //UpdateFill(count, 0);
        }
    }
    public void NewUpdateDealerPosition()
    {
        int count = TotalActiveDealer();
        //count++;
        Debug.LogWarning(" TotalActiveDealer" + count);
        switch (count)
        {
            case 1:
                break;
            case 2:
                NewUpdateFillWithId(0);
                NewUpdateFillWithId(1);
                break;
            case 3:
                NewUpdateFillWithId(0);
                NewUpdateFillWithId(1);
                NewUpdateFillWithId(2);
                break;
            case 4:
                NewUpdateFillWithId(0);
                NewUpdateFillWithId(1);
                NewUpdateFillWithId(2);
                NewUpdateFillWithId(3);
                break;
            case 5:
                NewUpdateFillWithId(0);
                NewUpdateFillWithId(1);
                NewUpdateFillWithId(2);
                NewUpdateFillWithId(3);
                break;
            default: break;
        }
        //UpdateFill(count, 0);
    }
    public void NewUpdateFillWithId(int index)
    {
        Debug.LogWarning("NewUpdateFillWithId ");
        var d = _dealers[index];
        Vector3 pos = d.transform.position;
        float xTarget = pos.x - 2.25f;
        d.SetDealerAndFillActive(false);
        Tween t = d.transform.DOMoveX(xTarget, 0.5f);
        t.OnStart(() =>
        {
            
        });
        t.OnUpdate(() =>
        {
            d._anchorPoint.DOMoveX(xTarget, 0.5f);
            d.UpdateFillPostion();
            d.UpdateCardStackPosition(d.transform.position);
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
                d.SetDealerAndFillActive(true);
            }
            t.Kill();
        });
    }

    public IEnumerator NextDealerCanUnlock()
    {
        var activeDealers = ActiveDealers();
        yield return new WaitUntil(() =>
        {
            foreach (var dealer in activeDealers)
            {
                if (dealer.isPlashCard)
                {
                    return false;
                }
            }
            return true;
        });
        var lastActiveDealer = _dealers.FindLast(dealer => dealer.Status == SlotStatus.Active || dealer.Status == SlotStatus.Locked);
        yield return new WaitForSeconds(1f);
        if (lastActiveDealer != null)
        {
            int nextID = ++lastActiveDealer.Id;
            if (nextID < _dealers.Count)
            {
                Dealer nextDealer = _dealers[nextID];
                var nextDealerData = DataAPIController.instance.GetDealerData(nextID);
                nextDealerData.status = SlotStatus.Locked;
                nextDealer.dealSlot.SettingBuyBtn(false);
                nextDealer.SetRewardActive(false);
                nextDealer.SetFillActive(false);
                Debug.LogWarning(" set reward active on new dealer can unlock");
                nextDealer.SetDealerLvelActive(false);
                nextDealer.gameObject.SetActive(true);
                nextDealer.transform.DOMoveX(lastActiveDealer.transform.position.x + 2.25f, 0f);
                nextDealer.dealSlot.SettingBuyBtn(true);
                nextDealer.gameObject.SetActive(true);
                int count = 4 % nextID;
            }
        }
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
