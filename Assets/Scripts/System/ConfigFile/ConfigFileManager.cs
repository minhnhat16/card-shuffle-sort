using System;
using System.Collections;
using UnityEngine;

public class ConfigFileManager : MonoBehaviour
{
    public static ConfigFileManager Instance;
    public bool isDone;
    [Header("CSV configs")]
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private PriceConfig priceConfig;
    [SerializeField] private DealerConfig dealerConfig;
    [SerializeField] private DealerPriceConfig dealerPriceConfig;
    [SerializeField] private ColorConfig colorConfig;
    [SerializeField] private SlotConfig slotConfig;
    [SerializeField] private ShopConfig shopConfig;
    [SerializeField] private ItemConfig itemConfig;
    [SerializeField] private DailyRewardConfig dailyConfig;
    [SerializeField] private SpinConfig spinConfig;
    [Header("Factory")]
    [SerializeField] private SoundFactory soundFactory;

    public LevelConfig LevelConfig { get => levelConfig; }
    public DealerPriceConfig DealerPriceConfig { get => dealerPriceConfig; }

    public PriceConfig PriceConfig { get => priceConfig; }
    //public PriceSlotConfig PriceSlotConfig { get => priceSlotConfig; }
    public ColorConfig ColorConfig { get => colorConfig; }  
    public SlotConfig SlotConfig { get => slotConfig; }

    public ShopConfig ShopConfig { get => shopConfig; }
    public ItemConfig ItemConfig { get => itemConfig; }
    public DailyRewardConfig DailyRewardConfig { get => dailyConfig; }
    public SpinConfig SpinConfig { get => spinConfig; }
    public SoundFactory SoundFactory { get => soundFactory; }
    public DealerConfig DealerConfig { get => dealerConfig; }

    private void Awake()
    {
        Instance = this;
    }

    public void Init(Action callback)
    {
        Debug.Log("(BOOT) // INIT CONFIG");
        StartCoroutine(WaitInit(callback));
    }
    // TODO: FIX PRICE SLOT CONFIG DOESNT INIT ON CONFIG FILE MANAGER INIT
    IEnumerator WaitInit(Action callback)
    {
        isDone = false;
        levelConfig = Resources.Load("Config/LevelConfig", typeof(ScriptableObject)) as LevelConfig;
        yield return new WaitUntil(() => levelConfig != null);
        //priceConfig = Resources.Load("Config/PriceConfig", typeof(ScriptableObject)) as PriceConfig;
        //yield return new WaitUntil(() => priceConfig != null);
        //priceSlotConfig = Resources.Load("Config/PriceSlotConfig", typeof(ScriptableObject)) as PriceSlotConfig;
        //yield return new WaitUntil(() => priceSlotConfig != null);
        slotConfig = Resources.Load("Config/SlotConfig", typeof(ScriptableObject)) as SlotConfig;
        yield return new WaitUntil(() => slotConfig != null);
        colorConfig = Resources.Load("Config/ColorConfig", typeof(ScriptableObject)) as ColorConfig;
        yield return new WaitUntil(() => colorConfig != null);
        //dealerConfig = Resources.Load("Config/DealerConfig", typeof(ScriptableObject)) as DealerConfig;
        //yield return new WaitUntil(() => dealerConfig != null);
        dealerPriceConfig = Resources.Load("Config/DealerPriceConfig", typeof(ScriptableObject)) as DealerPriceConfig;
        yield return new WaitUntil(() => dealerPriceConfig != null);
        //shopConfig = Resources.Load("Config/ShopConfig", typeof(ScriptableObject)) as ShopConfig;
        //yield return new WaitUntil(() => shopConfig != null);
        //itemConfig = Resources.Load("Config/ItemConfig", typeof(ScriptableObject)) as ItemConfig;
        //yield return new WaitUntil(() => itemConfig != null);
        dailyConfig = Resources.Load("Config/DailyRewardConfig", typeof(ScriptableObject)) as DailyRewardConfig;
        yield return new WaitUntil(() => dailyConfig != null);
        spinConfig = Resources.Load("Config/SpinConfig", typeof(ScriptableObject)) as SpinConfig;
        yield return new WaitUntil(() => spinConfig != null);
        soundFactory = Resources.Load("Factory/SoundFactory", typeof(ScriptableObject)) as SoundFactory;
        SoundManager.instance.Init();
        Debug.Log("(BOOT) // INIT CONFIG DONE");
        yield return new WaitUntil(() => soundFactory != null);
        yield return null;
        isDone = true;
        callback?.Invoke();
    }
}
