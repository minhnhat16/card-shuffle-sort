using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LableChooseDialog : BaseDialog
{
    public TabShop tabShop;
    public TabSkin tabSkin;
    public TabPlay tabPlay;
    public TabLeaderBoard tabLeaderBoard;
    public Text gold_lb;

    [HideInInspector]
    public UnityEvent<int> onGoldChanged = new UnityEvent<int>();
    private void OnEnable()
    {
        if (IngameController.instance.onGoldChanged != null)
        {
            onGoldChanged = IngameController.instance.onGoldChanged;
        }
        onGoldChanged.AddListener(GoldChange);
    }
    private void OnDisable()
    {
        onGoldChanged.RemoveListener(GoldChange);
    }
    public override void OnStartShowDialog()
    {
        base.OnStartShowDialog();
        gold_lb.text = DataAPIController.instance.GetGold().ToString();
        SelectPlayTab();
    }
    public void AddGoldButton()
    {
        SelectShopTab();
    }
    public void GoldChange(int gold)
    {
        gold_lb.text = gold.ToString();
    }
    public void SelectShopTab()
    {
        tabShop.OnClickTabOn();
        tabPlay.OnTabOff();
        tabLeaderBoard.OnTabOff();
        tabSkin.OnTabOff();
        PauseDialogOff();

    }
    public void SelectSkinTab()
    {
        tabSkin.OnClickTabOn();
        tabLeaderBoard.OnTabOff();
        tabPlay.OnTabOff();
        tabShop.OnTabOff();
        PauseDialogOff();

    }
    public void SelectPlayTab()
    {
        tabPlay.OnClickTabOn();
        tabShop.OnTabOff();
        tabLeaderBoard.OnTabOff();
        tabSkin.OnTabOff();
        PauseDialogOff();
    }
    public void SelectLeadBoardTab()
    {
        tabLeaderBoard.OnClickTabOn();
        tabPlay.OnTabOff();
        tabShop.OnTabOff();
        tabSkin.OnTabOff();
        PauseDialogOff();
    }
    public void SettingDialogButton()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClickSFX);
        DialogManager.Instance.ShowDialog(DialogIndex.SettingDialog, null);
    }
    public void DailyDialogButton()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClickSFX);
        DialogManager.Instance.ShowDialog(DialogIndex.DailyRewardDialog, null);

    }
    public void PauseDialogOff()
    {
        DialogManager.Instance.HideDialog(DialogIndex.SettingDialog);
    }
}
