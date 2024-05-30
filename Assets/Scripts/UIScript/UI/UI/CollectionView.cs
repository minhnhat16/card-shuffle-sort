using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CollectionView : BaseView
{
    [SerializeField] SpinCircle spiner;
    [SerializeField] Button button;

    [SerializeField] UnityEvent<bool> spinnedEvent = new UnityEvent<bool>();

    private void OnEnable()
    {
        spinnedEvent.AddListener(SpinDone);
        spiner = GetComponentInChildren<SpinCircle>();

    }
    private void OnDisable()
    {
        spinnedEvent?.RemoveListener(SpinDone);
    }
    public override void OnStartShowView()
    {
        base.OnStartShowView();
        spiner.spinnedEvent = spinnedEvent;
        NewDayCheck();
    }
    public override void OnEndShowView()
    {
        base.OnEndShowView();
    }
    private void Update()
    {

    }
    public void HideViewAfterSpinCouroutine()
    {
        StartCoroutine(HideViewAfterSpin());
    }

    public IEnumerator HideViewAfterSpin()
    {
        yield return new WaitUntil(() => spiner.IsSpining == false);
        yield return new WaitForSeconds(1f);
        //DialogManager.Instance.ShowDialog(DialogIndex.LabelChooseDialog);
    }   
    public void NewDayCheck()
    {
        //if (DayTimeController.instance.isNewDay)
        //{
        //    button.gameObject.SetActive(true);
        //}
        //else
        //{
        //    button.gameObject.SetActive(false);
        //}
    }
    public void SpinDone(bool isDone)
    {
        if(isDone)
        {
            var anim = GetComponentInChildren<SpinAnim>();
            //anim.SpinDoneAnim(() =>
            //{
            //    Debug.Log(" Spindone anim invoke");
            //    HideViewAfterSpinCouroutine();
            //});
        }
    }
}

