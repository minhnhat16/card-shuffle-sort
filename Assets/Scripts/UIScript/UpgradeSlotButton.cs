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

    [HideInInspector] public UnityEvent<bool> levelUpgraded = new();
    private void OnEnable()
    {
        upgradeButton.onClick.AddListener(OnClickUpgradeButton);
    }
    private void OnClickUpgradeButton()
    {
        int walletTotal = DataAPIController.instance.GetWalletByType(upgradeType);
        if (price > walletTotal) return;
        DataAPIController.instance.MinusWalletByType(price, upgradeType, (bool isDone) =>
        {
            levelUpgraded?.Invoke(isDone);
        });
    }

}