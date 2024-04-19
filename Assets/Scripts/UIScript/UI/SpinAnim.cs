using System;
using UnityEngine;

public class SpinAnim : BaseViewAnimation
{
    public Animator animator;
    private Action callback;
    public override void HideViewAnimation(Action callback)
    {
        this.callback = callback;
        animator.Play("SpinViewHide");
    }

    public override void ShowViewAnimation(Action callback)
    {
        this.callback = callback;
        animator.Play("SpinViewShow");
    }
    public void SpinDoneAnim(Action callback)
    {
        this.callback = callback;
        animator.Play("SpinDone");
    }
    public void ShowAnim()
    {
        callback?.Invoke();
    }

    public void HideAnim()
    {
        callback?.Invoke();
    }
    public void Clear()
    {
        Debug.Log("Clear in anim");
        callback?.Invoke();
    }
}
