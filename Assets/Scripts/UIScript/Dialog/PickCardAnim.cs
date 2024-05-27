using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickCardAnim : BaseDialogAnimation
{
    public Animator animator;
    private Action callback;
    public override void HideDialogAnimation(Action callback)
    {
        this.callback = callback;
        Debug.Log("HideDialogAnimation");
        animator.Play("PickCardHide");
    }
    public override void ShowDialogAnimation(Action callback)
    {
        this.callback = callback;
        Debug.Log("ShowDialogAnimation");
        animator.Play("PickCardShow");
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
