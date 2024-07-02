using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveAnim : BaseDialogAnimation
{
    public Animator animator;
    private Action callback;
    public override void HideDialogAnimation(Action callback)
    {
        this.callback = callback;
        //Debug.Log("ReviveHide");
        animator.Play("ReviveHide");
    }

    public override void ShowDialogAnimation(Action callback)
    {
        this.callback = callback;
        //Debug.Log("ReviveShow");
        animator.Play("ReviveShow");
    }

    public void ShowAnim()
    {
        callback?.Invoke();
    }

    public void HideAnim()
    {
        callback?.Invoke();
    }
    public void SetLoseCamOn()
    {
        //Debug.Log("Clear");
    }
    public void SetLoseCamOff()
    {
    }
}
