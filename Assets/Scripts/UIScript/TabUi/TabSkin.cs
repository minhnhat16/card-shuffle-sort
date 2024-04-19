using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabSkin: MonoBehaviour
{
    public GameObject tabOn;
    public GameObject tabOff;
    public Animator animator;

    public void OnClickTabOn()
    {
        tabOn.SetActive(true);
        tabOff.SetActive(false);
        animator.Play("TabOn");
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClickSFX);
        ViewParam viewParam = new();
        ViewManager.Instance.SwitchView(ViewIndex.WardrobeView,viewParam);
    }
    public void OnTabOff()
    {
        tabOn.SetActive(false);
        tabOff.SetActive(true);
    }
}
