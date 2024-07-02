using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ItemGrid : MonoBehaviour
{
    [SerializeField] private static int idShop = 1;
    [SerializeField] private int total;
    [SerializeField] private ShopItemTemplate _itemPrefab;
    [SerializeField] private List<ShopItemTemplate> _items;
    [SerializeField] ShopConfigRecord shopConfig = new ShopConfigRecord();
    [SerializeField] List<ItemConfigRecord> itemConfigs = new();
    [SerializeField] List<PriceConfigRecord> priceConfigRecords = new();

    private void Start()
    {
        shopConfig = ConfigFileManager.Instance.ShopConfig.GetRecordByKeySearch(idShop);
        itemConfigs = ConfigFileManager.Instance.ItemConfig.GetAllRecord();
        priceConfigRecords = ConfigFileManager.Instance.PriceConfig.GetAllRecord();
        InstantiateItems();
    }
    private void InstantiateItems()
    {
        bool isEnable;
        int id;
        int price;
        string spriteName;
        ItemType type;
        int total;
        //Debug.Log("Have price config" + idShop);

        foreach (var i in shopConfig.IdPrice)
        {
            GameObject item = Instantiate((Resources.Load("Prefab/UIPrefab/ShopItemTemplate", typeof(GameObject))), transform) as GameObject;
            if (item == null)
            {
                //Debug.LogError(" item == null");
            }
            else
            {
                var priceItem = priceConfigRecords[i];
                id = priceItem.IdItem;
                price = priceItem.Price;
                spriteName = itemConfigs[id].SpriteName;
                type = itemConfigs[id].Type;
                total = priceItem.Amount;
                isEnable = priceItem.Available;
                item.GetComponent<ShopItemTemplate>().SetupItem(id, price, spriteName, type, total, isEnable);
                _items.Add(item.GetComponent<ShopItemTemplate>());
            }
        }
    }
  
}
