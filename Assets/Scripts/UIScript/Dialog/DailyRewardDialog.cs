using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class DailyRewardDialog : BaseDialog
{
    public DailyClaimBtn claimBtn;
    public DailyGrid dailyGrid;

    [HideInInspector] public UnityEvent<bool> onClickDailyItem = new();
    [HideInInspector] public UnityEvent<bool> onClickClaim = new();
    [HideInInspector] public UnityEvent<bool> onClickAds = new();
    private void OnEnable()
    {
        onClickDailyItem?.AddListener(ClickDailyItem);
        onClickClaim?.AddListener(ClickClaimReward);
        onClickAds?.AddListener(OnClickAdsReward);
        claimBtn.SetButtonEvent(onClickClaim, onClickAds);
    }
    private void OnDisable()
    {
        onClickDailyItem.RemoveListener(ClickDailyItem);
        onClickClaim.RemoveListener(ClickClaimReward);
        onClickAds.RemoveListener(OnClickAdsReward);
    }
    public override void OnStartShowDialog()
    {
        base.OnStartShowDialog();
        //bool isClaimItem = DataAPIController.instance.GetIsClaimTodayData();
        //onClickDailyItem?.Invoke(isClaimItem);
        bool isCurrentAvailable = dailyGrid.currentDaily == null ? false : true;
        ClickDailyItem(isCurrentAvailable);
    }

    public void ClickDailyItem(bool isEnable)
    {
        Debug.Log("ClickDailyItem");
        claimBtn.enabled = true;
        if (isEnable)
        {
            Debug.Log($"Check Button type {isEnable} + claimbtn {claimBtn.gameObject.activeSelf}");
            claimBtn.CheckButtonType();
            claimBtn.gameObject.SetActive(isEnable);
        }
        else if(ZenSDK.instance.IsVideoRewardReady() && !isEnable)
        {
            //claimBtn.enabled = false;
            Debug.Log($"Check Button type {isEnable}");
            
            claimBtn.claim.gameObject.SetActive(false);
            claimBtn.ads.gameObject.SetActive(true);
        }
        else
        {
            claimBtn.gameObject.SetActive(false);
        }
    }
    public void ClickClaimReward(bool isClaim)
    {
        Debug.Log("ClickClaimReward");

        if (isClaim)
        {
            Debug.Log("claim reward successful");
            claimBtn.gameObject.SetActive(false);
            dailyGrid.currentDaily?.ItemClaim(isClaim);
        }
    }
    public void OnClickAdsReward(bool isAds)
    {
        if (isAds && ZenSDK.instance.IsVideoRewardReady())
        {
            Debug.Log("Ads reward showing");
            ZenSDK.instance.ShowVideoReward((isWatched) =>
            {
                if (isWatched) dailyGrid.currentDaily.ItemClaim(isWatched);
                else Debug.Log("Cant take item form video reward");
            });
        }
    }
    public void QuitButton()
    {
        DialogManager.Instance.HideDialog(dialogIndex, () =>
        {

            Debug.Log("quit button callback");
        });
    }
}
