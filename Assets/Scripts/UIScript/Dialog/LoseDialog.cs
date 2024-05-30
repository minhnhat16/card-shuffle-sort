using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoseDialog : BaseDialog
{
    [SerializeField] private Text score_lb;
    [SerializeField] private LoseDialogParam param;
    [SerializeField] private Camera boxCamera;
    private void Start()
    {
        param = new LoseDialogParam();
        score_lb = GetComponentInChildren<Text>();
    }
    public override void Setup(DialogParam dialogParam)
    {
        base.Setup(dialogParam);
    }
    public override void OnEndHideDialog()
    {
        base.OnEndHideDialog();
        ZenSDK.instance.ShowFullScreen();
    }
    public void HomeBtn()
    {
        SoundManager.instance.PlaySFX(SoundManager.SFX.UIClickSFX);

        DialogManager.Instance.HideDialog(dialogIndex);
        LoadSceneManager.instance.LoadSceneByName("Buffer", () =>
        {
            ZenSDK.instance.OnGameOver(param.score.ToString());
            //DialogManager.Instance.ShowDialog(DialogIndex.LabelChooseDialog,null, () =>
            //{
            //});
        });
    }
    public void RePlayBtn()
    {
        SoundManager.instance.PlaySFX(SoundManager.SFX.UIClickSFX);
       
        DialogManager.Instance.HideDialog(DialogIndex.LoseDialog, () =>
        {
            LoadSceneManager.instance.LoadSceneByName("Ingame", () =>
            {
                ViewManager.Instance.SwitchView(ViewIndex.GamePlayView, null, () =>
                {
                });
            });
        });
    }
 

}
