using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GamePlayView : BaseView
{
    //[HideInInspector] GamePlayAnim anim;
    [SerializeField] Text score_rb;
    [SerializeField] private int _changeGold;
    [SerializeField] private int gold;
    [SerializeField] private int gem;
    [SerializeField] private Text gold_lb;
    [SerializeField] private Text gem_lb;
    [SerializeField] private Text magnet_lb;
    [SerializeField] private Text bomb_lb;

    [SerializeField] Button magnet_btn;
    [SerializeField] Button bomb_Btn;
    [SerializeField] bool onMagnet;
    [SerializeField] bool onBomb;
    [HideInInspector]
    public UnityEvent<int> magnetItemEvent = new();
    [HideInInspector]
    public UnityEvent<int> bombItemEvent = new();

    public Text GoldLb { get { return gold_lb; } }
    public Text GemLB { get { return gem_lb; } }
    private void OnEnable()
    {
        //setGoldTextEvent = GridSystem.instance.setGoldTextEvent;
        //setGoldTextEvent.AddListener(ShowGoldAnim
        if (!IngameController.instance.isActiveAndEnabled) return;
        DataTrigger.RegisterValueChange(DataPath.GOLDINVENT, (data) =>
        {
            if (data == null) return;
            CurrencyWallet newData = data as CurrencyWallet;
            gold = newData.amount;
            gold_lb.text = gold.ToString();
        });
        DataTrigger.RegisterValueChange(DataPath.GEMINVENT, (data) =>
        {
            if (data == null) return;
            CurrencyWallet newData = data as CurrencyWallet;
            gem = newData.amount;
            gem_lb.text = gold.ToString();
        });
    }
    private void OnDisable()
    {
    }
    public override void OnStartShowView()
    {
        base.OnStartShowView();
    }
    public string CheckTotalItem(int total)
    {
        if (total > 0) return total.ToString();
        else return "0";
    }
    public override void OnStartHideView()
    {
        base.OnStartHideView();
    }
    public override void Setup(ViewParam viewParam)
    {
        base.Setup(viewParam);
        int gold = DataAPIController.instance.GetGold();
        int gem = DataAPIController.instance.GetGem();
        this.gold = gold;
        gold_lb.text = gold.ToString();
        gem_lb.text = gem.ToString();

    }
    private void Update()
    {
    }
    public void ShowGoldAnim(int gold)
    {
        _changeGold = gold;
        int calGold = _changeGold - this.gold;
        if (calGold == 0) return;

        this.gold = _changeGold;
        Debug.LogWarning("GOLD SHOW ANIM");
        gold_lb.text = gold.ToString();

    }
    public void ScoreChange(int score)
    {
        score_rb.text = score.ToString();
    }
    public void PauseButton()
    {
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClickSFX);
    }
   
   
    public void SettingButton()
    {
        PauseButton();
        SoundManager.Instance.PlaySFX(SoundManager.SFX.UIClickSFX_3);
        DialogManager.Instance.ShowDialog(DialogIndex.SettingDialog);
    }
    public void RateButton()
    {
        PauseButton();
        ZenSDK.instance.Rate();
    }
}
