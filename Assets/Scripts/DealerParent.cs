using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerParent : MonoBehaviour
{
    protected private const string path = "Prefabs/Dealer";
    public Vector3 spacing = new Vector3(-2, 0, 0);
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
        DataTrigger.RegisterValueChange(DataPath.SLOTDICT, (key) =>
        {
            UpdateFill();
        });
    }
    public void Init()
    {
        InitDealer();
    }
    public void InitDealer()
    {
        for (int i = 0; i < 4; i++) 
        {
            Dealer dealer = Instantiate(Resources.Load(path, typeof(Dealer)), transform) as Dealer;
            DealerData data = DataAPIController.instance.GetDealerData(i);
            dealer.Id = i;
            dealer.SetDealerAndFillActive(data.isUnlocked);
            dealer.transform.SetPositionAndRotation(spacing, Quaternion.identity);
            dealer.UpdateFillPostion();
            dealer.SetGoldGroupPosition();
            spacing += new Vector3(2, 0);
            _dealers.Add(dealer);
        }
    }
    private void UpdateFill()
    {
        for (int i = 0; i < _dealers.Count; i++)
        {
            if (_dealers[i].isActiveAndEnabled) _dealers[i].UpdateFillPostion();
        }
    }
}
