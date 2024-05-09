using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dealer:MonoBehaviour
{
    [SerializeField] private int upgradeLevel;

    public Slot dealSlot;
    public Image fillImg;
    public float fillOffset;
    public RectTransform dealerFill;
    public RectTransform goldGroup;
    public Transform fill;
    public Transform _anchorPoint;
    public UpgradeSlotButton upgrade_btn;
    [HideInInspector] public UnityEvent<bool> isUpgraded = new(); 
    public int UpgradeLevel { get { return upgradeLevel; } set { upgradeLevel = value; } }
    private void OnEnable()
    {
        isUpgraded = upgrade_btn.levelUpgraded;
        isUpgraded.AddListener(OnUpgradedDealer);
        StartCoroutine(DealerEvent());
    }

   

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
    public void PlayGoldAnim(int gold)
    {
        Debug.Log($"Play Gold Anim with amount {gold}");
    }
    public void PlayGemAnim(Action callback)
    {

    }
    private void OnUpgradedDealer(bool isUpgraded)
    {
        if (isUpgraded)
        {
            Debug.Log("OnUpgradedDealer");
            upgradeLevel++;
        }
    }
    public void SetGoldGroupPosition()
    {
        ScreenToWorld.Instance.SetWorldToAnchorView(dealerFill,goldGroup);
    }
    public void UpdateGoldAndGemToData(int gold, int gem)
    {
        DataAPIController.instance.AddGold(gold);
        DataAPIController.instance.AddGem(gem);
       //TODO : ADD GOLD AND GEM WHEN CLEARING CARD
       //TODO : ADD GOLD & GAM  ANIMATION
    }
    IEnumerator DealerEvent()
    {
        yield return new WaitUntil(() => IngameController.instance != null);
        dealSlot.expChanged = IngameController.instance.onExpChange;
        yield return new WaitUntil(() => IngameController.instance != null);
        dealSlot.gemCollected = IngameController.instance.onDealerClaimGold;
    }
}