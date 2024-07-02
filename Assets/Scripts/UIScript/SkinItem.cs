using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkinItem : MonoBehaviour
{
    [SerializeField] public bool isDisable;
    [SerializeField] private bool isOwned;
    [SerializeField] private int skinID;
    [SerializeField] private int price;
    [SerializeField] private string skinName;
    public List<Image> fruitImages;
    public Text skinName_lb;
    public Image disableMask;
    public Image unOwn;
    public Image Onwed;
    public Image equipedBG;
    public Image unquipedBG;
    public ConfirmButton confirmBtnType;
    public int SkinID { get => skinID; set => skinID = value; }
    public bool IsOwned { get => isOwned; set => isOwned = value; }
    public int Price { get => price; set => price = value; }
    public  Image OwnedImg { get => Onwed; set => Onwed = value; }
    public UnityEvent<bool> onClickAction = new UnityEvent<bool>();
    public UnityEvent<SkinItem> onEquipAction = new UnityEvent<SkinItem>();

    public void OnEnable()
    {
        onClickAction = confirmBtnType.onClickAction;
        onClickAction.AddListener(ButtonEvent);
    }
    public void InitSkin(int skinType, bool isOwned, bool isDisable,string skinname)
    {
        this.SkinID = skinType;
        this.isOwned = isOwned;
        this.isDisable = isDisable;
        this.skinName = skinname;
        skinName_lb.text= skinName;
        CheckSkinIsObtain(isOwned);
    }
    public void CheckSkinIsObtain(bool isObtain)
    {
        if (isObtain && !isDisable)
        {
            SetItemEquiped();
            //ADD BUY OR EQUIPCONDITION
        }
        else if (isObtain && isDisable)
        {
            SetItemUnquiped();
        }
        else
        {
            SetItemBuy();
        }
    }
    public void SetItemEquiped()
    {
        //Debug.Log("Skin name" + skinName);
        Onwed.gameObject.SetActive(true);
        disableMask.gameObject.SetActive(false);
        equipedBG.gameObject.SetActive(true);
        confirmBtnType.SwitchButtonType(ButtonType.Equiped);
    }
    public void SetItemUnquiped()
    {
        //Debug.Log("SKIN UNQUIPED");
        disableMask.gameObject.SetActive(false);
        Onwed.gameObject.SetActive(true) ;
        unOwn.gameObject.SetActive(false);
        unquipedBG.gameObject.SetActive(true);
        equipedBG.gameObject.SetActive(false);
        confirmBtnType.SwitchButtonType(ButtonType.Unquiped);

    }
    public void SetItemBuy()
    {
        //Debug.Log($"Skin name {skinName}");

        disableMask.gameObject.SetActive(true);
        Onwed.gameObject.SetActive(false) ;
        unOwn.gameObject.SetActive(true);
        if(price > 0)
        {
            confirmBtnType.SwitchButtonType(ButtonType.Buy);
            confirmBtnType.UpdatePriceLb(price.ToString());
        }
        else
        {
            confirmBtnType.SwitchButtonType(ButtonType.Ads);
        }
    }
    public void ButtonEvent(bool isClicked)
    {
        if (isClicked)
        {
            SoundManager.instance.PlaySFX(SoundManager.SFX.UIClickSFX);

            switch (confirmBtnType.Btntype)
            {
                case ButtonType.Ads:
                    //Debug.Log("WATCH ADS TO GET NEW SKIN");
                    BuyInvoke();
                    return;
                case ButtonType.Equiped:
                    //Debug.Log("SKIN IS EQUIPPING");
                    //MAKE AN ACTION ON WARDROBE VIEW TO INVOKE THE ANIM FLOATING TEXT "EQUIPED SKIN"
                    return;
                case ButtonType.Unquiped: //SWITCH CURRENT SKIN FROM ANOTHER TO THIS
                    //Debug.Log("SKIN EQUIPPED");
                    SetItemEquiped();
                    onEquipAction?.Invoke(this);
                    return;
                case ButtonType.Buy:
                    //Debug.Log("TRY TO BUY WITH AN AMOUNT OF GOLD");
                    BuyInvoke();
                    return;
                default:
                    return;
            }
        }
    }
    BuyConfirmDialogParam param = new BuyConfirmDialogParam();
    void BuyInvoke()
    {
        //Debug.Log("ONLICKBUYBUTTON");
        int goldHave = DataAPIController.instance.GetGold();
        int intCost = param.cost = Convert.ToInt32(price.ToString());
        param.onConfirmAction = () =>
        {

            DataAPIController.instance.MinusGoldWallet(intCost, null);
            string skinname = ConfigFileManager.Instance.ItemConfig.GetRecordByKeySearch(SkinID).SpriteName;
            InitSkin(SkinID, isOwned, true, skinname);
            
            SetItemUnquiped();
        };
        if ((confirmBtnType.Btntype.Equals(ButtonType.Buy)) && goldHave >= intCost)
        {
            param.plaintext = "DO YOU WANT TO BUY THIS PACK";
            param.cost_lb = intCost.ToString();
            DialogManager.Instance.ShowDialog(DialogIndex.BuyConfirmDialog, param);
        }
        else if (confirmBtnType.Btntype.Equals(ButtonType.Ads))
        {
            param.plaintext = "WATCH A VIDEO TO CLAIM THIS";
            DialogManager.Instance.ShowDialog(DialogIndex.BuyConfirmDialog, param);

        };
    }
}
public class SkinViewItemAction
{
    public Action onItemSelect;
    public Action onItemEquip;
}
