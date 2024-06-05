using DG.Tweening;
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
    [SerializeField] private SlotStatus status;
    //[SerializeField] private bool isUnlocked;
    public float fillOffset;
    public Vector3 fixedPosition;
    public Slot dealSlot;
    public Image fillImg;
    public Text level_lb;
    public RectTransform dealerFill;
    public RectTransform goldGroup;
    public RectTransform gemGroup;
    public RectTransform dealerLevel;
    public Transform _anchorPoint;
    public Transform _anchorLevel;
    public SpriteRenderer render;
    public UpgradeSlotButton upgrade_btn;
    [SerializeField] private DealerPriceConfigRecord dealerRec;
    [SerializeField] private SlotConfigRecord slotRec;

    [HideInInspector] public UnityEvent<bool> isUpgraded = new();

    public int UpgradeLevel { get { return upgradeLevel; } set { upgradeLevel = value; } }

    public int RewardGold { get => rewardGold; set => rewardGold = value; }
    public int RewardGem { get => rewardGem; set => rewardGem = value; }
    //public bool IsUnlocked { get => isUnlocked; set => isUnlocked = value; }
    public int Id { get => id; set => id = value; }
    public DealerPriceConfigRecord DealerRec { get => dealerRec; set => dealerRec = value; }
    public SlotConfigRecord SlotRec { get => slotRec; set => slotRec = value; }
    public SlotStatus Status { get => status; set => status = value; }

    private void OnEnable()
    {
        isUpgraded = upgrade_btn.levelUpgraded;
        isUpgraded.AddListener(OnUpgradedDealer);
        StartCoroutine(DealerEvent());
    }
    private void OnDisable()
    {
        isUpgraded.RemoveAllListeners();
    }
    private void UpdateDealerReward(object data)
    {
        Debug.LogWarning($"Is data null {data is null}");
        if (data == null) return;
        DealerData newData = (DealerData)data;
        if(newData.id == id)
        {
            Debug.LogWarning($"Update Dealer Reward {id}" );
            dealerRec = ConfigFileManager.Instance.DealerPriceConfig.GetRecordByKeySearch(newData.upgradeLevel);
            upgrade_btn.SetSlotButton(dealerRec.Cost, dealerRec.CurrencyType);
            RewardGem = dealerRec.LevelGem;
            RewardGold = dealerRec.LevelGold;
        }
       
    }
    public void Init()
    {
        //GET DATA AND CONFIG RECORD
        var data = DataAPIController.instance.GetDealerData(Id);
        upgradeLevel = DataAPIController.instance.GetDealerLevelByID(Id);
        dealerRec = ConfigFileManager.Instance.DealerPriceConfig.GetRecordByKeySearch(upgradeLevel);
        slotRec = ConfigFileManager.Instance.SlotConfig.GetRecordByKeySearch(id);
        RewardGem = dealerRec.LevelGem;
        RewardGold = dealerRec.LevelGold;
        upgrade_btn.SetSlotButton(dealerRec.Cost, dealerRec.CurrencyType);  
        dealSlot.status = status = data.status;
        
        if (status == SlotStatus.InActive)
        {
            gameObject.SetActive(false);
            SetDealerAndFillActive(false);
        }
        else if (status == SlotStatus.Locked)
        {
            isUpgraded = upgrade_btn.levelUpgraded;
            SetDealerAndFillActive(false);  
            dealSlot.SetSlotPrice(id, SlotRec.Price, slotRec.Currency);
            gameObject.SetActive(true);
            SetRender();
        }
        else
        {
            Debug.Log("Dealer is Active or can Unlock");
            SetDealerAndFillActive(true);
            dealSlot.SettingBuyBtn(false);
            SetRender();
        }
        if (dealSlot._cards.Count > 0)
        {
            var color = dealSlot.TopColor();
            Color c = ConfigFileManager.Instance.ColorConfig.GetRecordByKeySearch(color).Color;
            fillImg.color = c;
            fillImg.fillAmount += 0.1f * dealSlot._cards.Count;
        }
        UpdateFillPostion();
    }
    IEnumerator Start()
    {
        Debug.Log("Start Dealer" + id);
        yield return new WaitUntil(() => ConfigFileManager.Instance.DealerPriceConfig != null);
        Init();
        DataTrigger.RegisterValueChange(DataPath.DEALERDICT + $"{id}", UpdateDealerReward);
        level_lb.text = $"{UpgradeLevel}";
    }

    public void Update()
    {
        _anchorPoint.position = transform.position - new Vector3(0,1.7f,0);
        int cardCout = dealSlot._cards.Count;
        if (cardCout != 0) return;
        fillImg.fillAmount = Mathf.Lerp(fillImg.fillAmount, 0, 5f * Time.deltaTime);
        if (fillImg.fillAmount < 0.01f)
        {
            fillImg.fillAmount = 0;
        }
        if (SlotCamera.Instance is null) return;
        if (SlotCamera.Instance.isScalingCamera)
        {
            UpdateFillPostion();
            int count = SlotCamera.Instance.mulCount;
            float scaleValue = SlotCamera.Instance.ScaleValue[count];
            Tween tween = upgrade_btn.transform.DOScale(/*upgrade_btn.transform.localScale - */new Vector3(scaleValue, scaleValue, scaleValue), SlotCamera.Instance.Mul_Time);
            Tween dealerTween = dealerFill.DOScale(/*upgrade_btn.transform.localScale - */new Vector3(scaleValue, scaleValue, scaleValue), SlotCamera.Instance.Mul_Time);
            tween.OnComplete(() => tween.Kill());
            dealerTween.OnComplete(() => tween.Kill());
        }
    }
    public void UpdateFillPostion()
    {
        //TODO: IF CAMERA CHANGED , Change fill positon
        ScreenToWorld.Instance.SetWorldToCanvas(dealSlot.BuyBtn);
        ScreenToWorld.Instance.SetWorldToCanvas(dealerFill);
        ScreenToWorld.Instance.SetWorldToCanvas(dealerLevel);
        ScreenToWorld.Instance.SetWorldToCanvas(upgrade_btn.GetComponent<RectTransform>());
        SetCurrencyAnimPosition();
        Debug.Log("Update Fill Position");
        dealerLevel.transform.SetPositionAndRotation(_anchorLevel.position, Quaternion.identity);
        dealSlot.BuyBtn.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        dealerFill.transform.SetPositionAndRotation(_anchorPoint.position, Quaternion.identity);
        upgrade_btn.transform.SetPositionAndRotation(_anchorPoint.position + new Vector3(0, -0.75f), Quaternion.identity);
    }
    public void SetRender()
    {
        switch (status)
        {
            case SlotStatus.Active:
                render.sprite = SpriteLibControl.Instance.GetSpriteByName($"dealer{dealSlot.status}");
                return;
            case SlotStatus.Locked:
                render.sprite = SpriteLibControl.Instance.GetSpriteByName($"Locked");
                dealSlot.SettingBuyBtn(true);
                UpdateFillPostion();
                return;
            case SlotStatus.InActive:
                SetDealerAndFillActive(false);
                return;
            default:
                return;

        }
    }
    public void SetDealerAndFillActive(bool isActive)
    {
        gameObject.SetActive(isActive);
        dealerFill.gameObject.SetActive(isActive);
        upgrade_btn.gameObject.SetActive(isActive);
        dealerLevel.gameObject.SetActive(isActive);
        SetFillAndBtnToCanvas();
    }
    public void SetFillAndBtnToCanvas()
    {
        ScreenToWorld.Instance.SetWorldToCanvas(dealerFill);
        ScreenToWorld.Instance.SetWorldToCanvas(upgrade_btn.GetComponent<RectTransform>());
    }

    private void OnUpgradedDealer(bool isUpgraded)
    {
        if (isUpgraded)
        {
            Debug.Log("OnUpgradedDealer" +id);
            upgradeLevel++;
            DataAPIController.instance.SetDealerLevel(Id, UpgradeLevel);
            level_lb.text = $"{UpgradeLevel}";
        }
    }
    public void SetCurrencyAnimPosition()
    {
        //Debug.LogWarning("SET GOLD GROUP POSITION");
        RectTransform rectT = dealerFill;
        rectT.position += ScreenToWorld.Instance.ConvertPosition(rectT.position);
        ScreenToWorld.Instance.SetWorldToAnchorView(rectT, goldGroup);
        ScreenToWorld.Instance.SetWorldToAnchorView(rectT, gemGroup);
        //ScreenToWorld.Instance.SetWorldToCanvas(goldGroup);
        //ScreenToWorld.Instance.SetWorldToCanvas(gemGroup);

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