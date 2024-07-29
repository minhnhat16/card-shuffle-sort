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

            yield return null; // Spread initialization over multiple frames
        }

        if (!GameManager.instance.IsNewPlayer)
        {
            int count = 0;
            foreach (var dealer in _dealers)
            {
                if (dealer.Status != SlotStatus.InActive)
                {
                    count++;
                }
            }
            UpdateFill(count, 0);
        }
    }

    public IEnumerator NextDealerCanUnlock()
    {
        var activeDealers = ActiveDealers();
        Debug.Log("NexDealer can unlock start ");
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
        Debug.Log("NexDealer can unlock done");
        var lastActiveDealer = _dealers.FindLast(dealer => dealer.Status == SlotStatus.Active);
        yield return new WaitForSeconds(1f);
        if (lastActiveDealer != null)
        {
            int nextID = lastActiveDealer.Id + 1;
            if (nextID < _dealers.Count)
            {
                Dealer nextDealer = _dealers[nextID];
                var nextDealerData = DataAPIController.instance.GetDealerData(nextID);
                nextDealerData.status = SlotStatus.Locked;
                nextDealer.dealSlot.SettingBuyBtn(false);
                nextDealer.SetDealerAndFillActive(false);
                nextDealer.SetRewardActive(false);
                nextDealer.SetFillActive(false);
                nextDealer.SetDealerLvelActive(false);
                nextDealer.gameObject.SetActive(true);
              
                //nextDealer.Init();
                //nextDealer.dealSlot.Init();
                nextDealer.transform.DOMoveX(lastActiveDealer.transform.position.x + 3f, 0.1f)
                    .OnComplete(() => nextDealer.dealSlot.SettingBuyBtn(true));
                UpdateFill(4, 0.5f, null);
            }
        }
    }

    private void UpdateFill(int count, float time, Action callback = null)
    {
        StartCoroutine(UpdateFillCoroutine(count, time, callback));
    }

    private IEnumerator UpdateFillCoroutine(int count, float time, Action callback)
    {
        var allSlotData = DataAPIController.instance.AllSlotDataInDict(IngameController.instance.CurrentCardType);
        for (int i = 0; i < count; i++)
        {
            Dealer dealer = _dealers[i];
            Vector3 pos = dealer.transform.position;
            float xTarget = pos.x - 2.25f;
            if (dealer.Status == SlotStatus.Active || dealer.Status == SlotStatus.Locked)
            {
                dealer.gameObject.SetActive(false);
                dealer.gameObject.SetActive(true);
            }

            Tween t = dealer.transform.DOMoveX(xTarget, time);
            t.OnStart(() =>
            {
                if (dealer.Status != SlotStatus.Active)
                {
                    dealer.SetDealerLvelActive(false);
                    dealer.SetFillActive(false);
                    dealer.SetUpgradeButtonActive(false);
                    dealer.SetDealerLvelActive(false);
                    dealer.SetRewardActive(false);
                }
            });
            t.OnUpdate(() =>
            {
                dealer._anchorPoint.DOMoveX(xTarget, time);
                dealer.UpdateFillPostion();
                dealer.UpdateCardStackPosition(dealer.transform.position);

            });

            t.OnComplete(() =>
            {
                t.Kill();
            });

            yield return t.WaitForCompletion();

            if (dealer.Status == SlotStatus.Active)
            {
                dealer.gameObject.SetActive(true);
                dealer.SetUpgradeButtonActive(true);
                dealer.SetFillActive(true);
                dealer.SetDealerLvelActive(true);
                dealer.SetRewardActive(true);
                dealer.dealSlot.LoadCardData(allSlotData[i].currentStack);
                dealer.dealSlot.UpdateSlotState();
                dealer.fillImg.color = ConfigFileManager.Instance.ColorConfig.GetRecordByKeySearch(dealer.dealSlot.TopColor()).Color;
                dealer.fillImg.fillAmount = dealer.dealSlot._cards.Count * 0.1f;
                dealer.goldGroup.SetPositionWithParent(dealer.gameObject);
                dealer.gemGroup.SetPositionWithParent(dealer.gameObject);
                dealer.dealSlot.UpdateCardPositionX(dealer.dealSlot.Pos);
            }
            else if (dealer.Status == SlotStatus.Locked)
            {
                dealer.SetFillActive(false);
                dealer.SetUpgradeButtonActive(false);
                dealer.SetDealerLvelActive(false);
                dealer.SetRewardActive(false);
                dealer.goldGroup.SetPositionWithParent(dealer.gameObject);
                dealer.gemGroup.SetPositionWithParent(dealer.gameObject);
                dealer.gameObject.SetActive(true);
            }
            else
            {
                dealer.SetDealerLvelActive(false);
                dealer.SetFillActive(false);
                dealer.SetUpgradeButtonActive(false);
                dealer.SetDealerLvelActive(false);
                dealer.SetRewardActive(false);
            }
        }

        callback?.Invoke();
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
}
