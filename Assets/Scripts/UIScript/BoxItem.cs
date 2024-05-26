 using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoxItem : MonoBehaviour
{
    [HideInInspector] public bool isDisableWall;
    [SerializeField] private bool isOwned;
    [SerializeField] private int skinID;
    [SerializeField] private int price;
    public Text skinName_lb;
    public GameObject disableMask;
    public GameObject equipedBG;
    public GameObject unquipedBG;
    public ConfirmButton confirmBtnType;
    public int SkinID { get => skinID; set => skinID = value; }
    public bool IsOwned { get => isOwned; set => isOwned = value; }
    public int Price { get => price; set => price = value; }
    private UnityEvent<bool> onClickAction = new UnityEvent<bool>();
    [HideInInspector] public UnityEvent<BoxItem> onEquipActionBox = new();
    public void OnEnable()
    {
        onClickAction = confirmBtnType.onClickAction;
        onClickAction.AddListener(ButtonEvent);
    }
    public void InitSkin(int skinType, bool isOwned, bool isDisableWall)
    {
        this.SkinID = skinType;
        this.isOwned = isOwned;
        this.isDisableWall = isDisableWall;
        CheckSkinIsObtain(isOwned);
    }
    public void CheckSkinIsObtain(bool isObtain)
    {
        if (isObtain && !isDisableWall)
        {
            SetItemEquiped();
            //ADD BUY OR EQUIPCONDITION
        }
        else if (isObtain && isDisableWall)
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
        disableMask.SetActive(false);
        equipedBG.SetActive(true);
        confirmBtnType.SwitchButtonType(ButtonType.Equiped);
    }
    public void SetItemUnquiped()
    {
        Debug.Log("SKIN UNQUIPED");
        disableMask.SetActive(false);
        unquipedBG.SetActive(true);
        equipedBG.SetActive(false);
        confirmBtnType.SwitchButtonType(ButtonType.Unquiped);

    }
    public void SetItemBuy()
    {
        disableMask.gameObject.SetActive(true);
        if (price > 0)
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
            switch (confirmBtnType.Btntype)
            {
                case ButtonType.Ads:
                    Debug.Log("WATCH ADS TO GET NEW SKIN");
                    BuyInvoke();
                    return;
                case ButtonType.Equiped:
                    Debug.Log("SKIN IS EQUIPPING");
                    //MAKE AN ACTION ON WARDROBE VIEW TO INVOKE THE ANIM FLOATING TEXT "EQUIPED SKIN"
                    return;
                case ButtonType.Unquiped: //SWITCH CURRENT SKIN FROM ANOTHER TO THIS
                    Debug.Log("SKIN EQUIPPED");
                    SetItemEquiped();
                    onEquipActionBox.Invoke(this);
                    return;
                case ButtonType.Buy:
                    Debug.Log("TRY TO BUY WITH AN AMOUNT OF GOLD");
                    BuyInvoke();
                    return;
                default:
                    return;
            }
        }
    }
    readonly BuyConfirmDialogParam param = new();
    void BuyInvoke()
    {
        Debug.Log("ONLICKBUYBUTTON");
        int goldHave = DataAPIController.instance.GetGold();
        int intCost = Convert.ToInt32(price.ToString());
        param.onConfirmAction = () =>
        {

            DataAPIController.instance.MinusGoldWallet(intCost,null);
            InitSkin(SkinID, isOwned, true);

            SetItemUnquiped();
        };
        if ((confirmBtnType.Btntype.Equals(ButtonType.Buy)) && goldHave >= intCost)
        {
            param.cost = intCost;
            param.cost_lb = intCost.ToString();
            param.plaintext = "DO YOU WANT TO BUY THIS PACK";
            DialogManager.Instance.ShowDialog(DialogIndex.BuyConfirmDialog, param);
        }
        else if (confirmBtnType.Btntype.Equals(ButtonType.Ads))
        {
            param.plaintext = "WATCH A VIDEO TO CLAIM THIS";
            DialogManager.Instance.ShowDialog(DialogIndex.BuyConfirmDialog, param);
        };
    }
}
