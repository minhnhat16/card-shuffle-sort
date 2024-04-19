using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartRate : MonoBehaviour
{
    [SerializeField] private int idStar;
    [SerializeField] private bool isOn;
    [SerializeField] private Button starOn;
    [SerializeField] public Button startOff;
    [SerializeField] private Animator anim;
    Action callback;
    [HideInInspector]
    public UnityEvent<int> starEvent = new UnityEvent<int>();

    [HideInInspector]
    public int IDStar { get { return idStar; } set { idStar = value; } }
    [HideInInspector]
    public bool IsOn { get { return isOn; } set { isOn = value; } }
    private void OnEnable()
    {
        starEvent.AddListener(ButtonCLick);
    }
    public void StarOnClicked()
    {
        isOn = true;
        Debug.Log($"StartOnClicked {idStar}");
        starEvent?.Invoke(IDStar);
    }
    public void StarOffClicked()
    {
        isOn = false;
        Debug.Log($"StarOffClicked {idStar}");
        starEvent?.Invoke(IDStar);
    }
    public  void ButtonCLick(int id)
    {
        Debug.Log($"ButtonCLick {id}");

    }
    public void SetOnStar(bool isOn)
    {
        this.isOn = isOn;
        startOff.gameObject.SetActive(!isOn);
        starOn.gameObject.SetActive(isOn);
    }
    public void HideStarAnim()
    {
        anim.Play("StarHidee");
    }
    public void ConfirmStarRate(Action callback)
    {
        Debug.Log("ConfirmStarRate");
        this.callback = callback;
        anim.Play("RateStar");
    }
    /// <summary>
    //CALL BACK INVOKE WHEN STAR ON 
    /// </summary>
    public void CallBackInvoke()
    {
        if (!isOn) return;
        isOn = false;
        Debug.LogWarning("callbackinvoke");
        callback?.Invoke();
    }
}
