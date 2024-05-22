using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class DealerParent : MonoBehaviour
{
    protected private const string path = "Prefabs/Dealer";
    public Vector3 spacing = new Vector3(0, 0, 0);
    private List<Dealer> _dealers = new();
    public List<Dealer> Dealers { get { return _dealers; } set { _dealers = value; } }

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
            NextDealerCanUnlock();
        });
    }

    public void Init()
    {
        InitDealer();
    }

    public void InitDealer()
    {
        var currentCardType = IngameController.instance.CurrentCardType;
        var all = ConfigFileManager.Instance.SlotConfig.GetAllRecord();
        var dealersData = DataAPIController.instance.GetAllDealerData();
        for (int i = 0; i < dealersData.Count; i++)
        {
            var slotRecord = all[i];
            SlotData slotData = DataAPIController.instance.GetSlotDataInDict(i, currentCardType);
            Dealer dealer = Instantiate(Resources.Load<Dealer>(path), transform);
            DealerData dealerData = DataAPIController.instance.GetDealerData(i);
            dealer.Id = i;
            dealer.IsUnlocked = dealerData.isUnlocked;

            if (slotData.status == SlotStatus.Active)
            {
                dealer.SetDealerAndFillActive(true);
                dealer.transform.SetPositionAndRotation(spacing, Quaternion.identity);
                dealer.SetGoldGroupPosition();
                dealer.dealSlot.ID = i;
                dealer.dealSlot.LoadCardData(slotData.currentStack);
                dealer.dealSlot.SetSlotPrice(slotData.id, all[i].Price, all[i].Currency);
                spacing += new Vector3(2.5f, 0);
            }

           
            //if (dealer.IsUnlocked) transform.DOMoveX(transform.position.x - 2.5f, 0);

            if (slotRecord != null)
            {
                Debug.Log("(DEALER) SLOT HAVE PRICE SLOT CONFIG");
                int idSlot = slotRecord.ID;
                int price = slotRecord.Price;
                Currency type = slotRecord.Currency;
                dealer.dealSlot.SetSlotPrice(idSlot, price, type);
            }
            dealer.UpdateFillPostion();
            _dealers.Add(dealer);
        }
    }

    public void NextDealerCanUnlock()
    {
        Debug.Log("Next dealer can unlock");
        var d = _dealers.FindLast(dealer => dealer.IsUnlocked);
        if (d != null)
        {
            int nextID = d.Id + 1;
            if (nextID < _dealers.Count)
            {
                _dealers[nextID].dealSlot.status = SlotStatus.Locked;
                _dealers[nextID].SetDealerAndFillActive(false);
                _dealers[nextID].gameObject.SetActive(true);
                _dealers[nextID].Init();
            
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
    private void UpdateFill()
    {
        for (int i = 0; i < _dealers.Count; i++)
        {
            int index = i;  // Capture the current index for the closure
            Vector3 pos = _dealers[index].transform.position;
            t = _dealers[index].transform.DOMoveX(pos.x - 2, 1f).OnUpdate(() =>
            {
                _dealers[index]._anchorPoint.DOMoveX(pos.x - 2, 1f);
                _dealers[index].UpdateFillPostion();
            });
        }
    }
}
