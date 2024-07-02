using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class SkinGrid : MonoBehaviour
{
    [SerializeField] public static SkinGrid Instance;
    [SerializeField] private SkinItem crSkinItem;
    [SerializeField] private List<SkinItem> _skins;
    [SerializeField] private static int ShopSkinId = 2;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private UnityEvent<SkinItem> onEquipAction = new UnityEvent<SkinItem>();
    [SerializeField] private FloatingText floatingText;
    [SerializeField] private Image crSkinHead;

    [SerializeField] private List<ItemConfigRecord> itemConfig = new();
    [SerializeField] private List<PriceConfigRecord> priceConfig = new();
    [SerializeField] private ShopConfigRecord shopConfig = new();
    private List<int> playerData = new();
    private void Awake()
    {
        if (Instance != null) Instance = this;
    }
    private void Start()
    {
    }
    private void OnEnable()
    {
        ResetScroll();
    }
    private void OnDisable()
    {
    }

    public void SetupItem()
    {
        //Debug.Log("SetupItem SKin ");
        //Debug.Log("SetupItem 2" );
        itemConfig = ConfigFileManager.Instance.ItemConfig.GetAllRecord();
        priceConfig = ConfigFileManager.Instance.PriceConfig.GetAllRecord();
        shopConfig = ConfigFileManager.Instance.ShopConfig.GetRecordByKeySearch(ShopSkinId);
        InitiateSkinItem();
    }
    private void InitiateSkinItem()
    {
       
    }

    public void ResetScroll()
    {
        if (scrollRect != null)
        {
            // Reset the scroll position to the top
            scrollRect.normalizedPosition = new Vector2(0f, 1f);
        }
        else
        {
            //Debug.LogError("ScrollRect not assigned to the script.");
            return;
        }
    }
}
