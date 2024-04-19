using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GamePlayView : BaseView
{
    //[HideInInspector] GamePlayAnim anim;
    [SerializeField] Text score_rb;
    [SerializeField] private int _changeGold;
    [SerializeField] private int gold;
    [SerializeField] private Text gold_lb;
    [SerializeField] private Text diamond_lb;
    [SerializeField] private Text magnet_lb;
    [SerializeField] private Text bomb_lb;
    [SerializeField] private Animator goldAnim;

    [SerializeField] Button magnet_btn;
    [SerializeField] Button bomb_Btn;
    [SerializeField] bool onMagnet;
    [SerializeField] bool onBomb;
    [HideInInspector]
    public UnityEvent<int> magnetItemEvent = new();
    [HideInInspector]
    public UnityEvent<int> bombItemEvent = new();
    private void OnEnable()
    {
        //setGoldTextEvent = GridSystem.instance.setGoldTextEvent;
        //setGoldTextEvent.AddListener(ShowGoldAnim
        if (!IngameController.instance.isActiveAndEnabled) return;

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
        //int gold = DataAPIController.instance.GetGold();
        //_curGold = gold;
        //gold_lb.text = _curGold.ToString();
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
        goldAnim.gameObject.SetActive(true);
        goldAnim.Play("GoldAddShow");
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
