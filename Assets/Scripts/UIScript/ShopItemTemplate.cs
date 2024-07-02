using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemTemplate : MonoBehaviour
{
    [SerializeField] private int id ;
    [SerializeField] private Image backGround;
    [SerializeField] private Image itemImg;
    [SerializeField] private ItemType type;
    [SerializeField] private Text name_lb;
    [SerializeField] private Image ContainBox;
    [SerializeField] private int  totalItem;
    [SerializeField] private Text total_lb;
    [SerializeField] private int intCost;
    [SerializeField] private Text cost_lb;
    [SerializeField] private bool enable;
    [SerializeField] List<GameObject> buttonType = new();

    public ItemType Type { get => type; set => type = value; }
    public int IntCost { get => intCost; set => intCost = value; }
    public int TotalItem { get => totalItem; set => totalItem = value; }
    public Image ItemImg { get => itemImg; set => itemImg = value; }
    public Text Name_lb { get => name_lb; set => name_lb = value; }
    public Image ContainBox1 { get => ContainBox; set => ContainBox = value; }
    public Text Total_lb { get => total_lb; set => total_lb = value; }
    public Text Cost_lb { get => cost_lb; set => cost_lb = value; }
    public bool Enable { get => enable; set => enable = value; }
    
   public virtual void Start()
    {
       cost_lb.text = intCost.ToString();
       total_lb.text = "x" + totalItem.ToString();
    }
    BuyConfirmDialogParam param = new BuyConfirmDialogParam();
    public void CheckPrice (int price)
    {
        if(price < 0)
        {
            buttonType[0].SetActive(true);
            buttonType[1].SetActive(false);
        }
        else
        {
            buttonType[1].SetActive(true);
            buttonType[0].SetActive(false);
        }
    }
    public void SetupItem(int id, int intCost,string spriteName,ItemType type, int total, bool enable)
    {
        this.id = id;
        this.intCost = intCost;
        //item.ItemImg.SetNativeSize();
        this.type = type;
        this.Name_lb.text = type.ToString();
        this.totalItem =total;
        this.enable= enable;
        CheckPrice(intCost);
    }
    public void OnClickBuyButton()
    {
        //Debug.Log("ONLICKBUYBUTTON");
        SoundManager.instance.PlaySFX(SoundManager.SFX.UIClickSFX);
        int goldHave = DataAPIController.instance.GetGold();
        int intCost = Convert.ToInt32(cost_lb.text);
        param.onConfirmAction = () =>
        {
            
            //if (type == ItemType.GOLD)
            //{
            //    DataAPIController.instance.MinusGold(intCost);
            //    DataAPIController.instance.AddGold(totalItem);
            //    IngameController.instance.GoldChanged();
            //}
            //else if (type == ItemType.HAMMER || type == ItemType.ROTATE || type == ItemType.CHANGE)
            //{
            //    DataAPIController.instance.MinusGold(intCost);
            //    DataAPIController.instance.AddItemTotal(type.ToString(), totalItem);
            //    IngameController.instance.GoldChanged();
            //}
        };
        if (intCost <0) {
            param.plaintext = "WATCH A VIDEO TO CLAIM THIS";
            param.cost = intCost;
            DialogManager.Instance.ShowDialog(DialogIndex.BuyConfirmDialog, param);
        }
        else if (enable == true && goldHave >= intCost)
        {
            param.amount_lb = total_lb.text;
            param.cost_lb = cost_lb.text;
            param.plaintext = "DO YOU WANT TO BUY THIS PACK";
            DialogManager.Instance.ShowDialog(DialogIndex.BuyConfirmDialog  , param);
        }
    }
}
