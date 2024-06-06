using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UpgradeSlotButton : MonoBehaviour
{
    [SerializeField] private int price;
    [SerializeField] private Currency upgradeType;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Text lb_priceUpgrade;
    [HideInInspector] public UnityEvent<bool> levelUpgraded = new();
    [SerializeField] public Image[] images;
    private void OnEnable()
    {
        upgradeButton.onClick.AddListener(OnClickUpgradeButton);
    }
    private void OnDisable()
    {
        upgradeButton.onClick.RemoveAllListeners();
    }
    public void Start()
    {
        Debug.Log("Upgrade SLot button Start");
        lb_priceUpgrade = GetComponentInChildren<Text>();

    }
    public void SetSlotButton(int price, Currency upgradeType)
    {
        Debug.Log($"SetSlot Button {price} & {upgradeType}");
        this.price = price;
        this.upgradeType = upgradeType;
        lb_priceUpgrade.text = price.ToString();
        if(upgradeType == Currency.Gem)
        {
            SetImage(false);
        }
        else SetImage(true);
    }
    public void SetImage(bool isActive)
    {
        images[0].gameObject.SetActive(isActive);
        images[1].gameObject.SetActive(!isActive);
    }
    private void OnClickUpgradeButton()
    {
        Debug.Log("ON CLICK UPGRADE BUTTON");
        CurrencyWallet wallet = DataAPIController.instance.GetWalletByType(upgradeType);
        if (price > wallet.amount) return;
            DataAPIController.instance.MinusWalletByType(price, upgradeType, (bool isDone) =>
            {
                SoundManager.instance.PlaySFX(SoundManager.SFX.UpgradeSFX);
                levelUpgraded?.Invoke(isDone);
            });
     
    }

}