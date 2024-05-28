using UnityEngine;
using UnityEngine.UI;

public class ItemConfirmDialog : BaseDialog
{
    private ItemType type;
    private bool isAds;
    [SerializeField] Text tutorial_lb;

    [SerializeField] Button ads;
    [SerializeField] Button confirm;

    public Button Ads { get { return ads; } set { this.ads = value; } }
    public Button Confirm { get { return confirm; } set { this.confirm = value; } }
    private void OnEnable()
    {
        ads.onClick.AddListener(() => { PlayAds(); });
        confirm.onClick.AddListener(() => { ConfirmUsingItem(); });
    }
    private void OnDisable()
    {
        ads.onClick.RemoveAllListeners();
        confirm.onClick.RemoveAllListeners();
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
                confirm.gameObject.SetActive(false);
            }
            else //else show confirm to use
            {
                ads.gameObject.SetActive(false);
                confirm.gameObject.SetActive(true);
            }
        }

    }
    public override void OnStartShowDialog()
    {
        base.OnStartShowDialog();
        ItemCase(type);
    }
    // Start is called before the first frame update
    public void PlayAds()
    {
        ZenSDK.instance.ShowVideoReward((isWatched) =>
        {
            if (isWatched)
            {
                DataAPIController.instance.SetItemTotal(type,1);
                ConfirmUsingItem();
            }
            else 
            {
                Debug.LogWarning("Watch reward unsuccesfull");
            
            }
            ;
        });
    }
    public void ConfirmUsingItem()
    {
        //switch (type)
        //{
        //    case ItemType.CHANGE:
        //        Debug.Log("DESTROY CURRENT FRUIT ON GRAPPLING HOOK");
        //        IngameController.instance.ChangeItem();
        //        DialogManager.Instance.HideDialog(DialogIndex.ItemConfirmDialog, null);
        //        break;
        //    case ItemType.HAMMER:
        //        Debug.Log("CHOSE ONE FRUIT TO DESTROY IT");
        //        IngameController.instance.BursItem();
        //        DialogManager.Instance.HideDialog(DialogIndex.ItemConfirmDialog, null);
        //        break;
        //    case ItemType.ROTATE:
        //        Debug.Log("MAKING ALL FRUIT SHAKE");
        //        IngameController.instance.ShakeItem();
        //        DialogManager.Instance.HideDialog(DialogIndex.ItemConfirmDialog, null);
        //        break;
        //}
    }
    public void CancelUsingItem()
    {
        DialogManager.Instance.HideDialog(dialogIndex, () =>
        {
        });
    }
    void ItemCase(ItemType type)
    {
        //switch (type)
        //{
        //    case ItemType.CHANGE:
        //        tutorial_lb.text = "DESTROY CURRENT FRUIT ON GRAPLING HOOK";
        //        break;
        //    case ItemType.HAMMER:
        //        tutorial_lb.text = "CHOSE ONE FRUIT TO DESTROY IT!!";
        //        break;
        //    case ItemType.ROTATE:
        //        tutorial_lb.text = "USING TO SHAKE BOX MAKE AND FRUIT MOVE";
        //        break;
        //}
    }
}
