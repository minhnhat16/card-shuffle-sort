using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RateDialog : BaseDialog
{
    [SerializeField] StarList stars;
    [SerializeField] Button rateBtn;
    [SerializeField] Button remindBtn;

    [HideInInspector]
    public UnityEvent<bool> rateEvent = new UnityEvent<bool>();

    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }
    private void Awake()
    {
    }
    public override void OnStartShowDialog()
    {
        base.OnStartShowDialog();
       
    }
    public override void OnEndHideDialog()
    {
        base.OnEndHideDialog();

    }
    public void CloseButton()
    {
        DialogManager.Instance.HideDialog(dialogIndex, () =>
        {
            //Debug.Log("CloseButton");
        });
    }
    public void ReMindLaterBtn()
    {
        DialogManager.Instance.HideDialog(this.dialogIndex, () =>
        {
            //Debug.Log("ReMindLaterBtn");
        });
    }
    public void OnRateEvents(bool isRate)
    {
        rateBtn.gameObject.SetActive(isRate);
        remindBtn.gameObject.SetActive(!isRate);
    }
    public void RateButton()
    {
        //Debug.Log("RateButton");
    }
}
