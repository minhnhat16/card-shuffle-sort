using DG.Tweening;
using System.Collections;
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
    public Text gold_reward;
    public Text gem_reward;
    public RectTransform dealerFill;
    public RectTransform r_rewardGold;
    public RectTransform r_rewardGem;
    public RectTransform goldGroup;
    public RectTransform gemGroup;
    public RectTransform dealerLevel;
    public Transform _anchorPoint;
    public Transform _anchorLevel;
    public Transform _anchorReward;
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
        if (newData.id == id)
        {
            Debug.LogWarning($"Update Dealer Reward {id}");
            dealerRec = ConfigFileManager.Instance.DealerPriceConfig.GetRecordByKeySearch(newData.upgradeLevel);
            upgrade_btn.SetSlotButton(dealerRec.Cost, dealerRec.CurrencyType);
            RewardGem = dealerRec.LevelGem;
            RewardGold = dealerRec.LevelGold;
            RewardUpdate();
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
            RewardUpdate();

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
            //Debug.Log("Dealer is Active or can Unlock");
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
        _anchorPoint.position = transform.position - new Vector3(0, 1.7f, 0);
        int cardCout = dealSlot._cards.Count;
        if (cardCout != 0) return;
        fillImg.fillAmount = Mathf.Lerp(fillImg.fillAmount, 0, 5f * Time.deltaTime);
        RewardCourountine();
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
            Tween tween = upgrade_btn.transform.DOScale(new Vector3(scaleValue, scaleValue, scaleValue), SlotCamera.Instance.Mul_Time + 0.5f);
            Tween dealerTween = dealerFill.DOScale(new Vector3(scaleValue, scaleValue, scaleValue), SlotCamera.Instance.Mul_Time + 0.5f);
            r_rewardGem.DOScale(new Vector3(scaleValue, scaleValue, scaleValue), SlotCamera.Instance.Mul_Time + 0.5f);
            r_rewardGold.DOScale(new Vector3(scaleValue, scaleValue, scaleValue), SlotCamera.Instance.Mul_Time + 0.5f);

            tween.OnComplete(() => tween.Kill(true));
            dealerTween.OnComplete(() => tween.Kill(true));
        }
    }
    public void UpdateFillPostion()
    {
        //TODO: IF CAMERA CHANGED , Change fill positon
        var screen2world = ScreenToWorld.Instance;
        screen2world.SetWorldToCanvas(dealSlot.BuyBtn);
        screen2world.SetWorldToCanvas(dealerFill);
        screen2world.SetWorldToCanvas(dealerLevel);
        screen2world.SetWorldToCanvas(upgrade_btn.GetComponent<RectTransform>());
        screen2world.SetWorldToCanvas(r_rewardGem);
        screen2world.SetWorldToCanvas(r_rewardGold);

        //Debug.Log("Update Fill Position");
        dealerLevel.transform.SetPositionAndRotation(_anchorLevel.position, Quaternion.identity);
        dealSlot.BuyBtn.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        dealerFill.transform.SetPositionAndRotation(_anchorPoint.position, Quaternion.identity);
        upgrade_btn.transform.SetPositionAndRotation(_anchorPoint.position + new Vector3(0, -0.75f), Quaternion.identity);

        r_rewardGold.transform.SetPositionAndRotation(_anchorReward.position + new Vector3(0, 0.3f), Quaternion.identity);
        r_rewardGem.transform.SetPositionAndRotation(_anchorReward.position + new Vector3(0, -0.3f), Quaternion.identity);
    }
    public void SetRender()
    {
        switch (status)
        {
            case SlotStatus.Active:
                render.sprite = SpriteLibControl.Instance.GetSpriteByName($"dealer{dealSlot.status}");
                RewardUpdate();
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
    public void RewardCourountine()
    {
        StartCoroutine(RewardUpdating());
    }
    IEnumerator RewardUpdating()
    {
        if(dealSlot.status != SlotStatus.Active)
        {
            SetRewardActive(false);
            yield return null;
        }
        yield return new WaitUntil(() => dealSlot._cards.Count == 0);
        SetRewardActive(dealSlot._cards.Count == 0);
        //yield return new WaitUntil(() => dealSlot._cards.Count != 0);
        //SetRewardActive(true);

    }
    public void SetRewardActive(bool isActive)
    {
        gold_reward.text = GameManager.instance.DevideCurrency(rewardGold);
        gem_reward.text = GameManager.instance.DevideCurrency(rewardGem);

        r_rewardGold.gameObject.SetActive(isActive);
        r_rewardGem.gameObject.SetActive(isActive);
        //if (rewardGem <= 0 || isActive) r_rewardGem.gameObject.SetActive(false);
        if (rewardGem <= 0) r_rewardGem.gameObject.SetActive(false);
        //else r_rewardGem.gameObject.SetActive(isActive);
    }
    public void RewardUpdate()
    {
        gold_reward.text = GameManager.instance.DevideCurrency(rewardGold);
        gem_reward.text = GameManager.instance.DevideCurrency(rewardGem);

        bool isSlotActive = dealSlot.status == SlotStatus.Active;
        r_rewardGold.gameObject.SetActive(isSlotActive);
        if (rewardGem <= 0 && !isSlotActive) r_rewardGem.gameObject.SetActive(false);
        else r_rewardGem.gameObject.SetActive(true);

    }
    public void SetDealerAndFillActive(bool isActive)
    {
        gameObject.SetActive(isActive);
        dealerFill.gameObject.SetActive(isActive);
        upgrade_btn.gameObject.SetActive(isActive);
        dealerLevel.gameObject.SetActive(isActive);
        SetRewardActive(isActive);
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
            Debug.Log("OnUpgradedDealer" + id);
            upgradeLevel++;
            DataAPIController.instance.SetDealerLevel(Id, UpgradeLevel);
            level_lb.text = $"{UpgradeLevel}";
        }
    }
    public void SetCurrencyAnimPosition()
    {
        Vector3 pos = transform.position - new Vector3(0, 2, 0);
        //Debug.LogError("SET GOLD GROUP POSITION 1" + goldGroup.position);
        ScreenToWorld.Instance.SetWorldToAnchorView(pos, goldGroup);
        //Debug.LogError("SET GEM GROUP POSITION 1" + gemGroup.position);
        ScreenToWorld.Instance.SetWorldToAnchorView(pos, gemGroup);
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
    private void OnDestroy()
    {
        Destroy(goldGroup.gameObject);
        Destroy(gemGroup.gameObject);
    }
    IEnumerator DealerEvent()
    {
        yield return new WaitUntil(() => IngameController.instance != null);
        dealSlot.expChanged = IngameController.instance.onExpChange;
    }
}