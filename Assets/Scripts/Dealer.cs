﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dealer : MonoBehaviour
{
    [SerializeField] private int upgradeLevel;
    [SerializeField] private int id;
    [SerializeField] private int rewardGold;
    [SerializeField] private int rewardGem;
    [SerializeField] private bool isUnlocked;
    public float fillOffset;
    public Vector3 fixedPosition;
    public Slot dealSlot;
    public Image fillImg;
    public RectTransform dealerFill;
    public RectTransform goldGroup;
    public Transform _anchorPoint;
    public SpriteRenderer render;
    public UpgradeSlotButton upgrade_btn;
    [SerializeField] private DealerPriceConfigRecord dealerRec;
    [SerializeField] private SlotConfigRecord slotRec;

    [HideInInspector] public UnityEvent<bool> isUpgraded = new();

    public int UpgradeLevel { get { return upgradeLevel; } set { upgradeLevel = value; } }

    public int RewardGold { get => rewardGold; set => rewardGold = value; }
    public int RewardGem { get => rewardGem; set => rewardGem = value; }
    public bool IsUnlocked { get => isUnlocked; set => isUnlocked = value; }
    public int Id { get => id; set => id = value; }
    public DealerPriceConfigRecord DealerRec { get => dealerRec; set => dealerRec = value; }
    public SlotConfigRecord SlotRec { get => slotRec; set => slotRec = value; }

    private void OnEnable()
    {
        isUpgraded = upgrade_btn.levelUpgraded;
        isUpgraded.AddListener(OnUpgradedDealer);
        DataTrigger.RegisterValueChange(DataPath.DEALERDICT +$"{id}", UpdateDealerReward);
        StartCoroutine(DealerEvent());
    }

    private void UpdateDealerReward(object data)
    {
        if (data == null) return;
        //Debug.Log("UPDATE DEALER DATA" +id + "object" + data.ToString());
        //Dictionary<string, DealerData> newDataDict = (Dictionary<string, DealerData>)data;
        //newDataDict.TryGetValue(DataTrigger.ToKey(Id), out DealerData newData);
        DealerData newData = (DealerData)data;
        if(newData.id == id)
        {
            dealerRec = ConfigFileManager.Instance.DealerPriceConfig.GetRecordByKeySearch(newData.upgradeLevel);
            upgrade_btn.SetSlotButton(dealerRec.Cost, dealerRec.CurrencyType);
            RewardGem = dealerRec.LevelGem;
            RewardGold = dealerRec.LevelGold;
        }
       
    }
    public void Init()
    {
        upgradeLevel = DataAPIController.instance.GetDealerLevelByID(Id);
        dealerRec = ConfigFileManager.Instance.DealerPriceConfig.GetRecordByKeySearch(upgradeLevel);
        slotRec = ConfigFileManager.Instance.SlotConfig.GetRecordByKeySearch(id);
        RewardGem = dealerRec.LevelGem;
        RewardGold = dealerRec.LevelGold;
        upgrade_btn.SetSlotButton(dealerRec.Cost, dealerRec.CurrencyType);
        dealSlot.ID = id;
        if (!isUnlocked)
        {
            dealSlot.status = slotRec.Status;
            SetDealerAndFillActive(false);
            dealSlot.SetSlotPrice(id, slotRec.Price, slotRec.Currency);
            dealSlot.SettingBuyBtn(IsUnlocked);
        }
        else
        {
            SetRender();
        }

        if (dealSlot._cards.Count > 0)
        {
            var color = dealSlot.TopColor();
            Color c = ConfigFileManager.Instance.ColorConfig.GetRecordByKeySearch(color).Color;
            fillImg.color = c;
            fillImg.fillAmount += 0.1f * dealSlot._cards.Count;
        }
    }
    IEnumerator Start()
    {
        yield return new WaitUntil(() => ConfigFileManager.Instance.DealerPriceConfig != null);
        Init();
    }

    public void Update()
    {
        _anchorPoint.position = transform.position - new Vector3(0,1.75f,0);
        int cardCout = dealSlot._cards.Count;
        if (cardCout != 0) return;
        fillImg.fillAmount = Mathf.Lerp(fillImg.fillAmount, 0, 5f * Time.deltaTime);
        if (fillImg.fillAmount < 0.01f)
        {
            fillImg.fillAmount = 0;
        }
        if (SlotCamera.instance.isScalingCamera)
        {
            UpdateFillPostion();
            int count = SlotCamera.instance.mulCount;
            float scaleValue = SlotCamera.instance.ScaleValue[count];
            Tween tween = upgrade_btn.transform.DOScale(/*upgrade_btn.transform.localScale - */new Vector3(scaleValue, scaleValue, scaleValue), SlotCamera.instance.Mul_Time);
            Tween dealerTween = dealerFill.DOScale(/*upgrade_btn.transform.localScale - */new Vector3(scaleValue, scaleValue, scaleValue), SlotCamera.instance.Mul_Time);
            tween.OnComplete(() => tween.Kill());
            dealerTween.OnComplete(() => tween.Kill());
        }
    }
    public void UpdateFillPostion()
    {
        //TODO: IF CAMERA CHANGED , Change fill positon
        ScreenToWorld.Instance.SetWorldToCanvas(dealerFill);
        ScreenToWorld.Instance.SetWorldToCanvas(upgrade_btn.GetComponent<RectTransform>());
        Debug.Log("Update Fill Position");
        dealerFill.transform.SetPositionAndRotation(_anchorPoint.position, Quaternion.identity);
        upgrade_btn.transform.SetPositionAndRotation(_anchorPoint.position + new Vector3(0, -0.75f), Quaternion.identity);
    }
    public void SetRender()
    {
        switch (dealSlot.status)
        {
            case SlotStatus.Active:
                render.sprite = SpriteLibControl.Instance.GetSpriteByName($"dealer{dealSlot.status}");
                return;
            case SlotStatus.Locked:
                render.sprite = SpriteLibControl.Instance.GetSpriteByName($"Locked");
                dealSlot.SettingBuyBtn(true);
                return;
            case SlotStatus.InActive:
                SetDealerAndFillActive(false);
                return;
            default:
                return;

        }
    }
    public void SetDealerCanUnlock(bool isActive)
    {
        
    }
    public void SetDealerAndFillActive(bool isActive)
    {
        gameObject.SetActive(isActive);
        dealerFill.gameObject.SetActive(isActive);
        upgrade_btn.gameObject.SetActive(isActive);
    }
    public void PlayGoldAnim(int gold)
    {
        Debug.Log($"Play Gold Anim with amount {gold}");
    }

    private void OnUpgradedDealer(bool isUpgraded)
    {
        if (isUpgraded)
        {
            Debug.Log("OnUpgradedDealer" +id);
            upgradeLevel++;
            DataAPIController.instance.SetDealerLevel(Id, UpgradeLevel);
        }
    }
    public void SetGoldGroupPosition()
    {
        ScreenToWorld.Instance.SetWorldToAnchorView(dealerFill, goldGroup);
    }
    public void SetDealerSprite()
    {
        switch (dealSlot.status)
        {
            case SlotStatus.Active:

                render.sprite = SpriteLibControl.Instance.GetSpriteByName(dealSlot.status.ToString());
                break;
            case SlotStatus.InActive:
                dealSlot.SettingBuyBtn(false);
                render.sprite = SpriteLibControl.Instance.GetSpriteByName(dealSlot.status.ToString());
                break;
            case SlotStatus.Locked:
                dealSlot.SettingBuyBtn(true);
                render.sprite = SpriteLibControl.Instance.GetSpriteByName(dealSlot.status.ToString());
                break;
            default: break;
        }
    }
    IEnumerator DealerEvent()
    {
        yield return new WaitUntil(() => IngameController.instance != null);
        dealSlot.expChanged = IngameController.instance.onExpChange;
    }
}