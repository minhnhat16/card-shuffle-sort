using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class ReviveDialog : BaseDialog
{
    [SerializeField] private ReviveDialogParam param;
    [SerializeField] private Camera boxCamera;
    public override void Setup(DialogParam dialogParam)
    {
        base.Setup(dialogParam);
    }
    public override void OnStartShowDialog()
    {
        base.OnStartShowDialog();
        //score_lb.text = "Score: " + IngameController.instance.Score.ToString();
    }
    public override void OnEndShowDialog()
    {
        base.OnEndShowDialog();
    }
    public override void OnStartHideDialog()
    {
        base.OnEndHideDialog();
    }
    public override void OnEndHideDialog()
    {
        base.OnEndHideDialog();
    }
    public void RefuseBtn()
    {
        SoundManager.instance.PlaySFX(SoundManager.SFX.UIClickSFX);
        DialogManager.Instance.HideDialog(dialogIndex, () =>
        {
            DialogManager.Instance.HideDialog(dialogIndex);
            ZenSDK.instance.ShowFullScreen();
            LoadSceneManager.instance.LoadSceneByName("Buffer", () =>
            {
                //DialogManager.Instance.ShowDialog(DialogIndex.LabelChooseDialog, null, () =>
                //{
                //});
        });
        });
        }
    public void ReviveButton()
    {
        ZenSDK.instance.ShowVideoReward((isVideoDone) =>
        {
            //DialogManager.Instance.HideDialog(DialogIndex.ReviveDialog, () =>
            //{
                
            //});
        });
       
    }
}
