using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinTabButton : MonoBehaviour
{
    public GameObject tabOn;
    public GameObject tabOffBox;
    public Animator animator;
    [SerializeField]private GameObject skinGrid;

    public void StartTabOn()
    {
        tabOn.SetActive(true);
        tabOffBox.SetActive(true);
        //animator.Play("TabSkin");
    }
    public void OnClickTabOn()
    {
        tabOn.SetActive(true);
        tabOffBox.SetActive(true);
        animator.Play("TabSkin");
        skinGrid.GetComponentInChildren<Scrollbar>().value = 1;
        skinGrid.SetActive(true);
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClickSFX);
    }
    public void OnTabOff()
    {
        skinGrid.SetActive(false);
        tabOn.SetActive(false);
        tabOffBox.SetActive(false);
    }

}
