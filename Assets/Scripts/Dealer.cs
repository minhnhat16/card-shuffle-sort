using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dealer:MonoBehaviour
{
    public Slot dealSlot;
    public Image fillImg;
    public float fillOffset;
    public RectTransform dealerFill;

    public Transform fill;
    public Transform _anchorPoint;

    private int upgradeLevel;

    public int UpgradeLevel { get { return upgradeLevel; } set { upgradeLevel = value; } }
    public void Update()
    {
        int cardCout = dealSlot._cards.Count;
        if (cardCout !=0) return;
         fillImg.fillAmount = Mathf.Lerp(fillImg.fillAmount, 0, 5f * Time.deltaTime);
         if(fillImg.fillAmount < 0.01f)
        {
            fillImg.fillAmount = 0;
        }
    }
    public void UpdateFillPostion()
    {
        //TODO: IF CAMERA CHANGED , Change fill positon
        ScreenToWorld.Instance.SetWorldToCanvas(dealerFill);
        dealerFill.transform.SetPositionAndRotation(_anchorPoint.position, Quaternion.identity);    
    }
    public void SetDealerAndFillActive(bool isActive)
    {
        gameObject.SetActive(isActive);
        dealerFill.gameObject.SetActive(isActive);
    }
    public void PlayGoldAnim(Action callback)
    {

    }
    public void PlayGemAnim(Action callback)
    {

    }
    public void UpdateGoldAndGemToData(int gold, int gem)
    {
        DataAPIController.instance.AddGold(gold);
        DataAPIController.instance.AddGem(gem);
       //TODO : ADD GOLD AND GEM WHEN CLEARING CARD
       //TODO : ADD GOLD & GAM  ANIMATION
    }
}