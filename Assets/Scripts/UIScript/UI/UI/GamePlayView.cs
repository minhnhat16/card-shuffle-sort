using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GamePlayView : BaseView
{
    //[HideInInspector] GamePlayAnim anim;
    [SerializeField] private RectTransform anchor;
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

    [SerializeField] Button settingBtn;
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

    public RectTransform Anchor { get => anchor; set => anchor = value; }

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
            gold_lb.text = GameManager.instance.DevideCurrency(gold);
        });
        DataTrigger.RegisterValueChange(DataPath.GEMINVENT, (data) =>
        {
            if (data == null) return;
            CurrencyWallet newData = data as CurrencyWallet;
            gem = newData.amount;
            gem_lb.text = GameManager.instance.DevideCurrency(gem);
        });
        DataTrigger.RegisterValueChange(DataPath.MAGNET, (data) =>
        {
            if (data == null) return;
            ItemData newData = data as ItemData;
            if (newData.total > 0) magnet_lb.text = $"{newData.total}";
            else magnet_lb.text = "0";
        });
        DataTrigger.RegisterValueChange(DataPath.BOMB, (data) =>
         {
             if (data == null) return;
             ItemData newData = data as ItemData;
             if (newData.total > 0) bomb_lb.text = $"{newData.total}";
             else bomb_lb.text = "0";

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
        settingBtn.onClick.AddListener(SettingButton);
    }
    private void OnDisable()
    {
        bomb_Btn.onClick.RemoveListener(BomItemClick);
        magnet_btn.onClick.RemoveListener(MagnetItemClick);
        settingBtn.onClick.RemoveListener(SettingButton);

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
        IngameController.instance.SaveCardListToSLots();
        SlotPool.Instance.pool.DeSpawnAll();
    }
    public override void Setup(ViewParam viewParam)
    {
        base.Setup(viewParam);
        int gold = DataAPIController.instance.GetGold();
        int gem = DataAPIController.instance.GetGem();


        this.gold = gold;
        this.gem = gem;
        gold_lb.text = GameManager.instance.DevideCurrency(gold);
        gem_lb.text = GameManager.instance.DevideCurrency(gem);

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
        if (bombData.total <= 0) DialogManager.Instance.ShowDialog(DialogIndex.ItemConfirmDialog);
        else
        {
            Debug.Log("BOMB ITEM CLICKED ");
            IngameController.instance.onBombEvent?.Invoke(true);
            StartCoroutine(ButtonCouroutine(bomb_Btn));
            return;
        }
        bombItemEvent?.Invoke(false);
    }
    public void DealerCounter()
    {

    }
    public void PauseButton()
    {
        //SoundManager.instance.PlaySFX(SoundManager.SFX.UIClickSFX_3);
    }


    public void SettingButton()
    {
        PauseButton();
        //SoundManager.instance.PlaySFX(SoundManager.SFX.UIClickSFX_3);
        DialogManager.Instance.ShowDialog(DialogIndex.SettingDialog);
    }
    public void RateButton()
    {
        PauseButton();
        ZenSDK.instance.Rate();
    }
}
