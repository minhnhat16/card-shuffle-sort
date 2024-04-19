using System;
using System.Collections;
using UnityEngine;

public class ConfigFileManager : MonoBehaviour
{
    public static ConfigFileManager Instance;
    public bool isDone;
    [Header("CSV configs")]
    [SerializeField] private PriceConfig priceConfig;
    [SerializeField] private ColorConfig colorConfig;
    [SerializeField] private SlotConfig slotConfig;
    [SerializeField] private ShopConfig shopConfig;
    [SerializeField] private ItemConfig itemConfig;
    [SerializeField] private DailyRewardConfig dailyConfig;
    [SerializeField] private SpinConfig spinConfig;
    [Header("Factory")]
    [SerializeField] private SoundFactory soundFactory;


    public PriceConfig PriceConfig { get => priceConfig; }
    public ColorConfig ColorConfig { get => colorConfig; }
    public SlotConfig SlotConfig { get => slotConfig; }


    public ShopConfig ShopConfig { get => shopConfig; }
    public ItemConfig ItemConfig { get => itemConfig; }
    public DailyRewardConfig DailyRewardConfig { get => dailyConfig; }
    public SpinConfig SpinConfig { get => spinConfig; }
    public SoundFactory SoundFactory { get => soundFactory; }
    private void Awake()
    {
        Instance = this;
    }

    public void Init(Action callback)
    {
        Debug.Log("(BOOT) // INIT CONFIG");
        StartCoroutine(WaitInit(callback));
    }

    IEnumerator WaitInit(Action callback)
    {
        isDone = false;
        priceConfig = Resources.Load("Config/PriceConfig", typeof(ScriptableObject)) as PriceConfig;
        yield return new WaitUntil(() => priceConfig != null);
        colorConfig = Resources.Load("Config/ColorConfig", typeof(ScriptableObject)) as ColorConfig;
        yield return new WaitUntil(() => colorConfig != null);
        shopConfig = Resources.Load("Config/ShopConfig", typeof(ScriptableObject)) as ShopConfig;
        yield return new WaitUntil(() => shopConfig != null);
        itemConfig = Resources.Load("Config/ItemConfig", typeof(ScriptableObject)) as ItemConfig;
        yield return new WaitUntil(() => itemConfig != null);
        slotConfig = Resources.Load("Config/SlotConfig", typeof(ScriptableObject)) as SlotConfig;
        yield return new WaitUntil(() => slotConfig != null);
        dailyConfig = Resources.Load("Config/DailyRewardConfig", typeof(ScriptableObject)) as DailyRewardConfig;
        yield return new WaitUntil(() => dailyConfig != null);
        spinConfig = Resources.Load("Config/SpinConfig", typeof(ScriptableObject)) as SpinConfig;
        yield return new WaitUntil(() => spinConfig != null);
        soundFactory = Resources.Load("Factory/SoundFactory", typeof(ScriptableObject)) as SoundFactory;
        SoundManager.Instance.Init();
        Debug.Log("(BOOT) // INIT CONFIG DONE");
        yield return new WaitUntil(() => soundFactory != null);
        yield return null;
        isDone = true;
        callback?.Invoke();
    }
}
