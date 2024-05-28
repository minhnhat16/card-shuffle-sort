using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GamePlayView : BaseView
{
    //[HideInInspector] GamePlayAnim anim;
    [SerializeField] private int _changeGold;
    [SerializeField] private int gold;
    [SerializeField] private int gem;
    [SerializeField] private Text gold_lb;
    [SerializeField] private Text gem_lb;
    [SerializeField] private Text magnet_lb;
    [SerializeField] private Text bomb_lb;
    [SerializeField] private Text curentCard_lb;
    [SerializeField] private Text maxCard_lb;
    [SerializeField] private Text timeCouter;

    [SerializeField] Button magnet_btn;
    [SerializeField] Button bomb_Btn;
    [SerializeField] bool onMagnet;
    [SerializeField] bool onBomb;
    [HideInInspector]
    public UnityEvent<bool> magnetItemEvent = new();
    [HideInInspector]
    public UnityEvent<bool> bombItemEvent = new();

    public Text GoldLb { get { return gold_lb; } }
    public Text GemLB { get { return gem_lb; } }
    private void OnEnable()
    {
        //setGoldTextEvent = GridSystem.instance.setGoldTextEvent;
        //setGoldTextEvent.AddListener(ShowGoldAnim
        //if (!IngameController.instance.isActiveAndEnabled) return;
        DataTrigger.RegisterValueChange(DataPath.GOLDINVENT, (data) =>
        {
            if (data == null) return;
            CurrencyWallet newData = data as CurrencyWallet;
            gold = newData.amount;
            gold_lb.text = DevideCurrency(gold);
        });
        DataTrigger.RegisterValueChange(DataPath.GEMINVENT, (data) =>
        {
            if (data == null) return;
            CurrencyWallet newData = data as CurrencyWallet;
            gem = newData.amount;
            gem_lb.text = DevideCurrency(gem);
        });
        DataTrigger.RegisterValueChange(DataPath.MAGNET, (data) =>
        {
            if (data == null) return;
            ItemData newData = data as ItemData;
            bomb_lb.text = $"{newData.total}";

        });
        DataTrigger.RegisterValueChange(DataPath.BOMB, (data) =>
         {
             if (data == null) return;
             ItemData newData = data as ItemData;
             bomb_lb.text = $"{newData.total}";

         });
        DataTrigger.RegisterValueChange(DataPath.LASTSAVETIME, (data) =>
        {
            if (data == null) return;
            string newData = data as string;

        });
        DataTrigger.RegisterValueChange(DataPath.MAXCARDPOOL, (data) =>
        {
            if (data == null) return;
            int newData = (int)data;

        });
        bomb_Btn.onClick.AddListener(BomItemClick);
        magnet_btn.onClick.AddListener(MagnetItemClick);

    }
    private void OnDisable()
    {
        bomb_Btn.onClick.RemoveListener(BomItemClick);
        magnet_btn.onClick.RemoveListener(MagnetItemClick);
    }
    public override void OnStartShowView()
    {
        base.OnStartShowView();
        StartCoroutine(GetItemFormData());
    }
    public string CheckTotalItem(int total)
    {
        if (total > 0) return total.ToString();
        else return "0";
    }
    public override void OnStartHideView()
    {
        base.OnStartHideView();
    }
    public override void Setup(ViewParam viewParam)
    {
        base.Setup(viewParam);
        int gold = DataAPIController.instance.GetGold();
        int gem = DataAPIController.instance.GetGem();


        this.gold = gold;
        this.gem = gem;
        gold_lb.text = DevideCurrency(gold);
        gem_lb.text = DevideCurrency(gem);

    }
    public void SetTimeCounter(DateTime time)
    {
        int minute = time.Minute;
        int second = time.Second;
        timeCouter.text =  $" 500 in {minute}:{second}";
    }
    IEnumerator GetItemFormData()
    {
        yield return new WaitUntil(() => DataAPIController.instance.GetItemData(ItemType.Bomb) is not null);
        ItemData bombTotal = DataAPIController.instance.GetItemData(ItemType.Bomb);
        ItemData magnetTotal = DataAPIController.instance.GetItemData(ItemType.Magnet);
        bomb_lb.text = $"{bombTotal.total}";
        magnet_lb.text = $"{magnetTotal.total}";
    }
    public string DevideCurrency(int currency)
    {
        if (currency < 10000) return currency.ToString();
        else
        {
            currency /= 1000;
            currency.ToString();
            return $"{currency}k";
        }
    }
    public void ShowGoldAnim(int gold)
    {
        _changeGold = gold;
        int calGold = _changeGold - this.gold;
        if (calGold == 0) return;

        this.gold = _changeGold;
        Debug.LogWarning("GOLD SHOW ANIM");
        gold_lb.text = gold.ToString();

    }
   
    public IEnumerator ButtonCouroutine(Button button)
    {
        yield return new WaitForSeconds(1f);
        button.interactable = true;
    }
    public void MagnetItemClick()
    {
        magnet_btn.interactable = false;
        var magnetData = DataAPIController.instance.GetItemData(ItemType.Magnet);
        if (magnetData.total < 0) DialogManager.Instance.ShowDialog(DialogIndex.ItemConfirmDialog);
        else
        {
            IngameController.instance.onMagnetEvent?.Invoke(true);
            StartCoroutine(ButtonCouroutine(magnet_btn));
            return;
        }
        magnetItemEvent.Invoke(false);
    }
    public void BomItemClick()
    {
        bomb_Btn.interactable = false;
        var bombData = DataAPIController.instance.GetItemData(ItemType.Bomb);
        if (bombData.total < 0) DialogManager.Instance.ShowDialog(DialogIndex.ItemConfirmDialog);
        else
        {
            Debug.Log("BOMB ITEM CLICKED ");
            IngameController.instance.onBombEvent?.Invoke(true);
            StartCoroutine(ButtonCouroutine(bomb_Btn));
            return;
        }
        bombItemEvent?.Invoke(false);
    }
    public void PauseButton()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClickSFX);
    }


    public void SettingButton()
    {
        PauseButton();
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClickSFX_3);
        DialogManager.Instance.ShowDialog(DialogIndex.SettingDialog);
    }
    public void RateButton()
    {
        PauseButton();
        ZenSDK.instance.Rate();
    }
}
