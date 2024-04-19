using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TabLeaderBoard : MonoBehaviour
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
        ViewManager.Instance.SwitchView(ViewIndex.RankView);
    }
    public void OnTabOff()
    {
        tabOn.SetActive(false);
        tabOff.SetActive(true);
    }
}
