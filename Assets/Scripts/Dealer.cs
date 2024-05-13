using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Dealer:MonoBehaviour
{
    [SerializeField] private int upgradeLevel;
    [SerializeField] private int id;
    [SerializeField] private int rewardGold;
    [SerializeField] private int rewardGem;
    public Slot dealSlot;
    public Image fillImg;
    public float fillOffset;
    public RectTransform dealerFill;
    public RectTransform goldGroup;
    public Transform fill;
    public Transform _anchorPoint;
    public Vector3 fixedPosition;
    public UpgradeSlotButton upgrade_btn;
    [SerializeField]private DealerConfigRecord record;
    [HideInInspector] public UnityEvent<bool> isUpgraded = new();

    public int Id { get; set; }
    public int UpgradeLevel { get { return upgradeLevel; } set { upgradeLevel = value; } }

    public int RewardGold { get => rewardGold; set => rewardGold = value; }
    public int RewardGem { get => rewardGem; set => rewardGem = value; }

    private void OnEnable()
    {
        isUpgraded = upgrade_btn.levelUpgraded;
        isUpgraded.AddListener(OnUpgradedDealer);
        DataTrigger.RegisterValueChange(DataPath.DEALERDICT, UpdateDealerReward);
        StartCoroutine(DealerEvent());
    }

    private void UpdateDealerReward(object data)
    {
        if (data == null) return;
        Debug.Log("UPDATE DEALER DATA" );
        DealerData newData = data as DealerData;
        record = ConfigFileManager.Instance.DealerConfig.GetRecordByKeySearch(newData.upgradeLevel);
        upgrade_btn.SetSlotButton(record.Cost, record.CurrencyType);
        RewardGem = record.LevelGem;    
        RewardGold = record.LevelGold;
    }

    IEnumerator Start()
    {
        upgradeLevel = DataAPIController.instance.GetPlayerLevel();
        yield return new WaitUntil(() => ConfigFileManager.Instance.DealerConfig != null);
        record = ConfigFileManager.Instance.DealerConfig.GetRecordByKeySearch(upgradeLevel);
        RewardGem = record.LevelGem;
        RewardGold = record.LevelGold;
        upgrade_btn.SetSlotButton(record.Cost, record.CurrencyType);
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
        ScreenToWorld.Instance.SetWorldToCanvas(upgrade_btn.GetComponent<RectTransform>());
        dealerFill.transform.SetPositionAndRotation(_anchorPoint.position, Quaternion.identity);
        upgrade_btn.transform.SetPositionAndRotation(_anchorPoint.position + new Vector3(0,-0.75f), Quaternion.identity);
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
            Debug.Log("OnUpgradedDealer");
            upgradeLevel++;
            DataAPIController.instance.SetDealerLevel(id, UpgradeLevel);
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
    }
}