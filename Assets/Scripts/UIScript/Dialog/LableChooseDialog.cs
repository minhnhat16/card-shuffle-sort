using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LableChooseDialog : BaseDialog
{
    public List<LableTab> lableList;
    public Text gold_lb;
    public Text gem_lb;

    private int gold;
    private int gem;
    [SerializeField] Button btn_Setting;
    [HideInInspector]
    public UnityEvent<Lable> onClickedRate = new();
    public UnityEvent<Lable> onClickedHome = new();
    public UnityEvent<Lable> onClickedSpin = new();
    public UnityEvent<Lable> onClickedCollection = new();
    private void OnEnable()
    {
        btn_Setting.onClick.AddListener(SettingDialogButton);
        onClickedRate = lableList[0].onChooseLable;
        onClickedHome = lableList[1].onChooseLable;
        onClickedSpin = lableList[2].onChooseLable;
        onClickedCollection = lableList[3].onChooseLable;
        onClickedRate.AddListener(RateClicked);
        onClickedHome.AddListener(HomeClicked);
        onClickedSpin.AddListener(SpinClicked);
        onClickedCollection.AddListener(CollectionClicked);
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
    }
    private void OnDisable()
    {
        //onGoldChanged.RemoveListener(GoldChange);
        onClickedRate.RemoveListener(RateClicked);
        onClickedHome.RemoveListener(HomeClicked);
        onClickedSpin.RemoveListener(SpinClicked);
        onClickedCollection.RemoveListener(CollectionClicked);
    }
    public override void OnStartShowDialog()
    {
        base.OnStartShowDialog();
        gold = DataAPIController.instance.GetGold();
        gem = DataAPIController.instance.GetGem();
        gold_lb.text = GameManager.instance.DevideCurrency(gold);
        gem_lb.text = GameManager.instance.DevideCurrency(gem);
        lableList[(int)Lable.Home].OnButtonClicked();
    }
    void HomeClicked(Lable lable)
    {
        if (lable != Lable.Home) return;
        SwitchButtonChose(lable);
        MainScreenViewParam param = new MainScreenViewParam();
        ViewManager.Instance.SwitchView(ViewIndex.MainScreenView, param, () =>
        {

        });
    }
    void RateClicked(Lable lable)
    {
        if (lable != Lable.Rate) return;
        SwitchButtonChose(lable);

        DialogManager.Instance.ShowDialog(DialogIndex.RateDialog, null, () =>
        {
           var main =  (MainScreenView)ViewManager.Instance.currentView;
            main.SetLevelPanelIs(false); 
        });
    }
    void SpinClicked(Lable lable)
    {
        if (lable != Lable.Spin) return;
        SwitchButtonChose(lable);
        DialogManager.Instance.ShowDialog(DialogIndex.SpinDialog,null, () =>
        {
            if(ViewManager.Instance.currentView.viewIndex == ViewIndex.MainScreenView)
            {
                var main = ViewManager.Instance.currentView as MainScreenView;
                main.SetLevelPanelIs(false);
            }
        });

    }
    void CollectionClicked(Lable lable)
    {
        if (lable != Lable.Collection) return;
        SwitchButtonChose(lable);
        ViewManager.Instance.SwitchView(ViewIndex.CollectionView);
    }
    void SwitchButtonChose(Lable lable)
    {
        foreach (LableTab tab in lableList)
        {
            if (tab.type != lable) tab.OnButtonUnchose();
        }
    }
    public void SettingDialogButton()
    {
        SoundManager.instance.PlaySFX(SoundManager.SFX.UIClickSFX);
        SettingParam param = new();
        param.isMainScreen = true;
        DialogManager.Instance.ShowDialog(DialogIndex.SettingDialog, param, null);
    }
    public void DailyDialogButton()
    {
        SoundManager.instance.PlaySFX(SoundManager.SFX.UIClickSFX);
        DialogManager.Instance.ShowDialog(DialogIndex.DailyRewardDialog, null);

    }
    public void PauseDialogOff()
    {
        DialogManager.Instance.HideDialog(DialogIndex.SettingDialog);
    }
}
