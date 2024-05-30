using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxTabButton : MonoBehaviour
{
    public GameObject tabOn;
    public GameObject tabOffSkin;
    public Animator animator;
    public GameObject boxGrid;
 
    public void OnClickTabOn()
    {
        tabOn.SetActive(true);
        tabOffSkin.SetActive(true);
        //animator.Play("TabSkin");
        boxGrid.GetComponentInChildren<ScrollRect>().verticalScrollbar.value = 1;
        boxGrid.SetActive(true);
        SoundManager.instance.PlaySFX(SoundManager.SFX.UIClickSFX);
    }
    public void OnTabOff()
    {
        boxGrid.SetActive(false);
        tabOn.SetActive(false);
        tabOffSkin.SetActive(false);
    }
}
