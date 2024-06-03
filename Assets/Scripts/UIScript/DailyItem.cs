using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DailyItem : MonoBehaviour
{
    public List<GameObject> backgrounds = new List<GameObject>();
    public Image itemImg;
    public int intAmount;
    public int day;
    public DailyReward itemName;
    public Text day_lb;
    public Text Amount_lb;
    public IEDailyType currentType;
    public Button daily_btn;
    [SerializeField] public UnityEvent<bool> onClickDailyItem = new();
    [SerializeField] public UnityEvent<bool> onItemClaim = new();

    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        onClickDailyItem.RemoveAllListeners();
        onItemClaim.RemoveAllListeners();
    }
    private void Start()
    {
        var parent = FindObjectOfType<DailyRewardDialog>();
        if (parent != null)
        {
            onClickDailyItem = parent.onClickDailyItem;
            onItemClaim = parent.onClickClaim;
        }
    }
    public void Init(IEDailyType type, int amount, int day, string spriteName, DailyReward itemName)
    {
        SwitchType(type);
        SetAmountLb(amount);
        SetDayLB(day);
        SetItemImg(spriteName);
        SetItemNameType(itemName);
    }
    public void SetItemImg(string spriteName)
    {
        Debug.Log(itemName);
        itemImg.sprite = SpriteLibControl.Instance.GetSpriteByName(spriteName);
    }
    public void SetItemNameType(DailyReward itemName)
    {
        this.itemName = itemName;
    }
    public void SetDayLB(int day)
    {
        //Debug.Log($"Set day lb {day}");
        this.day = day;
        day_lb.text += $"Day {day}";
    }
    public void SetAmountLb(int amount)
    {
        //Debug.Log($"Set amount lb {amount}");
        intAmount = amount;
        Amount_lb.text = amount.ToString();
    }
    public virtual void SwitchType(IEDailyType type)
    {
        currentType = type;
        daily_btn.enabled = true;
        switch (type)
        {
            case IEDailyType.Available:
                backgrounds[0].SetActive(true);
                backgrounds[1].SetActive(false);
                backgrounds[2].SetActive(false);
                daily_btn.enabled = true;
                break;
            case IEDailyType.Unavailable:
                backgrounds[1].SetActive(true);
                //daily_btn.gameObject.SetActive(false);
                break;
            case IEDailyType.Claimed:
                backgrounds[1].SetActive(false);
                backgrounds[0].SetActive(false);
                backgrounds[2].SetActive(true);
                daily_btn.enabled = false;
                Amount_lb.gameObject.SetActive(false);
                itemImg.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }
    public void SwitchItemType(DailyReward item)
    {

        switch (item)
        {
            case DailyReward.Gold_S:
                Debug.Log("Reward: Small Gold");
                // Add logic for small gold reward
                DataAPIController.instance.AddGold(intAmount);
                break;
            case DailyReward.Gold_M:
                // Add logic for medium gold reward
                Debug.Log("Reward: Medium Gold");
                DataAPIController.instance.AddGold(intAmount);
                break;
            case DailyReward.Bomb:
                Debug.Log("Reward: Bomb");
                // Add logic for bomb reward
                DataAPIController.instance.AddItemTotal(ItemType.Bomb,intAmount);

                break;
            case DailyReward.Gold_L:
                Debug.Log("Reward: Large Gold");
                // Add logic for large gold reward
                DataAPIController.instance.AddGold(intAmount);
                break;
            case DailyReward.Gem:
                Debug.Log("Reward: Gem");
                // Add logic for gem reward
                DataAPIController.instance.AddGem(intAmount);
                break;
            case DailyReward.Magnet:
                Debug.Log("Reward: Magnet");
                // Add logic for magnet reward
                DataAPIController.instance.AddItemTotal(ItemType.Magnet, intAmount);
                break;
            case DailyReward.Bonus:
                Debug.Log("Reward: Bonus");
                // Add logic for bonus reward
                DataAPIController.instance.AddItemTotal(ItemType.Magnet, 10);
                DataAPIController.instance.AddItemTotal(ItemType.Magnet, 10);
                DataAPIController.instance.AddGold(1500);
                DataAPIController.instance.AddGem(20);
                break;
            default:
                Debug.LogWarning("Unexpected reward type: " + item);
                break;
        }
    }
    public void ItemClaim(bool isClaim)
    {
        if (isClaim)
        {
            SwitchType(IEDailyType.Claimed);
            DataAPIController.instance.SetDailyData(day--, currentType);
            SwitchItemType(itemName);
        }
    }
    public void CheckItemAvailable()
    {
        Debug.Log("On Click Daily Item");
        if (currentType == IEDailyType.Available)
        {
            Debug.Log("On Click Daily Item" + IEDailyType.Available);
            onClickDailyItem?.Invoke(true);
        }
        else
        {
            Debug.Log("On Click Daily Item" + IEDailyType.Unavailable);
            bool checkVidReward = ZenSDK.instance.IsVideoRewardReady();
            onClickDailyItem?.Invoke(checkVidReward);
        }
    }
    public virtual void OnClickDailyItem()
    {
        CheckItemAvailable();
    }
}