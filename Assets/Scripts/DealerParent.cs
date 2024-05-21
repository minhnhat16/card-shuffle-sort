using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerParent : MonoBehaviour
{
    protected private const string path = "Prefabs/Dealer";
    public Vector3 spacing = new Vector3(0, 0, 0);
    private List<Dealer> _dealers = new();
    public List<Dealer> _Dealers { get { return _dealers; } set { _dealers = value; } }
    public Dealer GetDealerByID(int id)
    {
        return _dealers[id] == null? null : _dealers[id];
    }
    public void SetDealerByID(int id, Dealer dealer)
    {
        if (dealer == null || id > 4) return;
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
        for (int i = 0; i < 4; i++) 
        {
            var slotRecord = all[i];
            SlotData slotData = DataAPIController.instance.GetSlotDataInDict(i, currentCardType) ?? null;
            Dealer dealer = Instantiate(Resources.Load(path, typeof(Dealer)), transform) as Dealer;
            DealerData dealerData = DataAPIController.instance.GetDealerData(i);
            dealer.Id = i;
            dealer.IsUnlocked = dealerData.isUnlocked;
            dealer.SetDealerAndFillActive(dealerData.isUnlocked);
            dealer.transform.SetPositionAndRotation(spacing, Quaternion.identity);
            dealer.UpdateFillPostion();
            dealer.SetGoldGroupPosition();
            dealer.dealSlot.ID = i;
            dealer.dealSlot.LoadCardData(slotData.currentStack);
            spacing += new Vector3(2, 0);
            if (slotRecord != null)
            {
                Debug.Log("(DEALER) SLOT HAVE PRICE SLOT CONFIG");
                int idSlot = slotRecord.ID;
                int price = slotRecord.Price;
                Currency type = slotRecord.Currency;
                dealer.dealSlot.SetSlotPrice(idSlot, price, type);
            }
            _dealers.Add(dealer);
        }
    }
    public void NextDealerCanUnlock()
    {
        var d = _dealers.Find(dealer => dealer.dealSlot.status == SlotStatus.Active);
        Dealer nextDealer = _dealers[d.Id++];
        nextDealer.SetDealerAndFillActive(true);
        var record = ConfigFileManager.Instance.SlotConfig.GetRecordByKeySearch(d.Id++);
        nextDealer.dealSlot.status = record.Status;
        nextDealer.dealSlot.SetSprite();
    }   
    private void UpdateFill()
    {
        for (int i = 0; i < _dealers.Count; i++)
        {
            if (_dealers[i].isActiveAndEnabled) _dealers[i].UpdateFillPostion();
        }
    }
}
