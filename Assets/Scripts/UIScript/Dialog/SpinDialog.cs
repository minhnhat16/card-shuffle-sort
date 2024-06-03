using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpinDialog : BaseDialog
{
    [SerializeField] private Button spinBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private SpinCircle circle;


    public override void OnStartShowDialog()
    {
        base.OnStartShowDialog();
        spinBtn.onClick?.AddListener(SpinCircle);
        exitBtn.onClick?.AddListener(() =>
        {
            Debug.Log("ONCLICK EXIT");
            DialogManager.Instance.HideDialog(dialogIndex);
        });
    }
    public override void OnEndHideDialog()
    {
        base.OnEndHideDialog();
    }
    public void SpinCircle()
    {
        circle.SpinningCircle();
    }
    
}
