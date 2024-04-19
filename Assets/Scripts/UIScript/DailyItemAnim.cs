using System;
using UnityEngine;

public class DailyItemAnim : MonoBehaviour
{
    [SerializeField] Animator animator;
    private Action callback;

    public void DailyShow(Action callback)
    {
        this.callback = callback;
        animator.Play("DailyItemShow");
    }

    public void DailyHide(Action callback)
    {
        this.callback = callback;
        animator.Play("DailyItemHide");
    }
    public void ShowAnim()
    {
        callback?.Invoke();
    }
    public void HideAnim()
    {
        callback?.Invoke();
    }
}
