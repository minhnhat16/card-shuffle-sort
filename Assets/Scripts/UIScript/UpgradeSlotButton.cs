using System.Collections;
using System.Collections.Generic;
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
    private void OnEnable()
    {
        upgradeButton.onClick.AddListener(OnClickUpgradeButton);
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

    }
    private void OnClickUpgradeButton()
    {
        Debug.Log("ON CLICK UPGRADE BUTTON");
        int walletTotal = DataAPIController.instance.GetWalletByType(upgradeType);
        if (price > walletTotal) return;
        DataAPIController.instance.MinusWalletByType(price, upgradeType, (bool isDone) =>
        {
            levelUpgraded?.Invoke(isDone);
        });
    }

}