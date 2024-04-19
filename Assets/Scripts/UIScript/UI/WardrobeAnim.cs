using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WardrobeAnim : BaseViewAnimation
{
    public Animator animator;
    private Action callback;
    public override void HideViewAnimation(Action callback)
    {
        this.callback = callback;
        animator.Play("WardrobeHide");
    }
    public override void ShowViewAnimation(Action callback)
    {
        this.callback = callback;
        animator.Play("WardrobeShow");
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
