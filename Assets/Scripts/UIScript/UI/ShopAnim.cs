using System;
using UnityEngine;

public class ShopAnim : BaseViewAnimation
{
    public Animator animator;
    private Action callback;
    public override void HideViewAnimation(Action callback)
    {
        this.callback = callback;
        animator.Play("ShopViewHide");
    }

    public override void ShowViewAnimation(Action callback)
    {
        this.callback = callback;
        animator.Play("ShopViewShow");
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
        callback?.Invoke();
    }
}
