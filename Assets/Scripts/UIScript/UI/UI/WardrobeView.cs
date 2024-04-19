using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WardrobeView : BaseView
{
    [SerializeField] private BoxTabButton boxTab;
    [SerializeField] private SkinTabButton skinTab;
    [SerializeField]  private SkinGrid skinGrid;
    [SerializeField] private BoxSkinGrid boxSkin;
    [SerializeField] private Image currentSkin;
    [SerializeField] private FloatingText  floatingText;
    public override void Setup(ViewParam viewParam)
    {
        base.Setup(viewParam);
    }
    public override void OnInit()
    {
        SetUpView();
    }
    public void SetUpView()
    {
        skinGrid.SetupItem();
        boxSkin.SetupBoxGrid();
    }
    public override void OnStartShowView()
    {
        base.OnStartHideView();
        StartTabOn();
    }
    public void StartTabOn()
    {
        skinGrid.gameObject.SetActive(true);
        skinTab.StartTabOn();
        boxTab.OnTabOff();
    }
    public void SelectBoxTab()
    {
        boxTab.OnClickTabOn();
        skinTab.OnTabOff();
    }
    public void SelectSkinTab()
    {
        skinTab.OnClickTabOn();
        boxTab.OnTabOff();
    }

}
