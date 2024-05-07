using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerParent : MonoBehaviour
{
    protected private const string path = "Prefabs/Dealer";
    public Vector3 spacing = new Vector3(-2, 0, 0);
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
            dealer.SetDealerAndFillActive(data.isUnlocked);
            dealer.transform.SetPositionAndRotation(spacing, Quaternion.identity);
            dealer.UpgradeLevel = 1;
            dealer.UpdateFillPostion();
            spacing += new Vector3(2, 0);
        }
    }
}
