using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickCardDialog : BaseDialog
{
    public CardPicker premium;
    public CardPicker free;
    public PickCardParam param;

    public UnityEvent<int> premiumCardChose = new();
    public UnityEvent<int> freeCardChose = new();
    private void OnEnable()
    {
        premiumCardChose = premium.cardChose;
        premiumCardChose.AddListener(PremiumChosen);
        freeCardChose = free.cardChose;
        freeCardChose.AddListener(FreeChosen);

    }

    private void FreeChosen(int arg0)
    {
        premium.gameObject.SetActive(false);
        free.Claimbtn.gameObject.SetActive(false);
        free.PlayClaimAnim(() =>
        {
            //Debug.Log("PlayClaimAnim INVOKED");
            DialogManager.Instance.HideDialog(DialogIndex.PickCardDialog, () =>
            {
                Player.Instance.isAnimPlaying = false;
                Player.Instance.isDealBtnActive = false;
            });
        });
    }

    private void PremiumChosen(int arg0)
    {
        free.gameObject.SetActive(false);
        premium.Claimbtn.gameObject.SetActive(false);
        premium.PlayClaimAnim(() =>
        {
            Debug.Log("PlayClaimAnim INVOKED");
            DialogManager.Instance.HideDialog(DialogIndex.PickCardDialog, () =>
            {
                Player.Instance.isAnimPlaying = false;
                Player.Instance.isDealBtnActive = false;
            });
        });
    }

    public override void Setup(DialogParam dialogParam)
    {
        param = (PickCardParam)dialogParam;
        var cardType = IngameController.instance.CurrentCardType;
        premium.Color = param.premium;
        free.Color = param.free;
        premium.Init(CardPickerType.Premium, param.premium, cardType);
        free.Init(CardPickerType.Free, param.free, cardType);
        if (!premium.isActiveAndEnabled)
        {
            free.transform.DOMoveX(0,0);
        }
    }
    public override void OnStartShowDialog()
    {
        base.OnStartShowDialog();

    }
}
