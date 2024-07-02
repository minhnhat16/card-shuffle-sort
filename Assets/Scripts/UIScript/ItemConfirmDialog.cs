using UnityEngine;
using UnityEngine.UI;

public class ItemConfirmDialog : BaseDialog
{
    private ItemType type;
    private bool isAds;
    [SerializeField] Text tutorial_lb;
    [SerializeField] Text price_lb;

    [SerializeField] Button ads;
    [SerializeField] Button buy;
    [SerializeField] int price;
    public Button Ads { get { return ads; } set { this.ads = value; } }
    public Button Confirm { get { return buy; } set { this.buy = value; } }
    private void OnEnable()
    {
        ads.onClick.AddListener(() => { PlayAds(); });
        buy.onClick.AddListener(() => { PurchaseItem(); });
    }
    private void OnDisable()
    {
        ads.onClick.RemoveAllListeners();
        buy.onClick.RemoveAllListeners();
    }
    public override void Setup(DialogParam dialogParam)
    {
        base.Setup(dialogParam);
        if (dialogParam != null)
        {
            ItemConfirmParam param = (ItemConfirmParam)dialogParam;
            type = param.type;
            isAds = param.isAds;
            //name = param.name;
            //check item have enoughf to show ads btn
            if (isAds)
            {
                ads.gameObject.SetActive(true);
                buy.gameObject.SetActive(false);
            }
            else //else show confirm to use
            {
                ads.gameObject.SetActive(false);
                buy.gameObject.SetActive(true);
            }
        }

    }
    public override void OnStartShowDialog()
    {
        base.OnStartShowDialog();
        ItemCase(type);
        price = ZenSDK.instance.GetConfigInt($"price+{type}", 3000);
        price_lb.text = price.ToString();
    }
    // Start is called before the first frame update
    public void PlayAds()
    {
        ZenSDK.instance.ShowVideoReward((isWatched) =>
        {
            if (isWatched)
            {
                DataAPIController.instance.SetItemTotal(type, 1);
                PurchaseItem();
            }
            else
            {
                //Debug.LogWarning("Watch reward unsuccesfull");
            }
            ;
        });
    }
    public void PurchaseItem()
    {
        int wallet = DataAPIController.instance.GetGold();
        if(wallet>= price)
        {
            //Play successfully buy sound
            DataAPIController.instance.MinusGoldWallet(price, (isDone) =>
            {
                DataAPIController.instance.AddItemTotal(type, 1);
            });
        }
        else
        {
            //play unsuccessfull sound;
        }
    }
    public void CancelUsingItem()
    {
        DialogManager.Instance.HideDialog(dialogIndex, () =>
        {
        });
    }
    void ItemCase(ItemType type)
    {
        switch (type)
        {
            case ItemType.Bomb:
                tutorial_lb.text = "RANDOMLY CLEAR ALL CARD IN ONE SLOT";
                break;
            case ItemType.Magnet:
                tutorial_lb.text = "RAMDOMLY CHOOSE ONE COLOR TO CLEAR";
                break;
            default:
                tutorial_lb.text = "SOME THING WENT WRONG";
                break;
        }
    }
}
