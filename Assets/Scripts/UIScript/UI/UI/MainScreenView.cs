using DanielLochner.Assets.SimpleScrollSnap;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenView : BaseView
{
    //public int totalGold;
    //public TextMeshProUGUI gold_lb;
    [SerializeField] private Button playBtn;
    [SerializeField] private Button dailyReward;
    [SerializeField] private Button prevBtn;
    [SerializeField] private Button nextBtn;

    [SerializeField] SimpleScrollSnap levelScroll;
    [SerializeField] private DynamicContent dynamicContent;
    [SerializeField] private LevelPanel levelPanel;
    private void OnEnable()
    {
        playBtn.onClick.AddListener(OnPlayButton);
        dailyReward.onClick.AddListener(OnDailyReward);
    }

   
    private void OnDisable()
    {
        playBtn.onClick.RemoveListener(OnPlayButton);
    }

    public override void Setup(ViewParam viewParam)
    {
        base.Setup(viewParam);
        dynamicContent.Init();
    }
   
    private void OnDailyReward()
    {
        DialogManager.Instance.ShowDialog(DialogIndex.DailyRewardDialog, null);
    }

    public override void OnInit(Action callback)
    {
        levelPanel.Init(callback);
       
    }

    private void OnPlayButton()
    {
        //Debug.Log("OnPlayButton");
        int levelLoad = 0;
        //SlotCamera.instance.gameObject.SetActive(true);
        //Debug.Log("Current card type" + levelLoad);
        LevelItem item =  levelPanel.GetLeveItem(levelLoad);
        bool isUnlocked = item.CheckUnlock();
        if (isUnlocked)
        {
            DataAPIController.instance.SetCurrentCardType((CardType)levelLoad, () =>
            {
                DialogManager.Instance.HideAllDialog();
            });
            LoadSceneManager.instance.LoadSceneByName("Ingame", () =>
            {
                GameManager.instance.LoadIngameSence(() =>
                {
                    GameManager.instance.SetupTutorial();
                    ViewManager.Instance.SwitchView(ViewIndex.GamePlayView, null, () =>
                    {
                    });

                });
               
            });
            Destroy(item.gameObject);
        }
        else
        {
            playBtn.interactable = true;
            var mainScreenAnim =(MainScreenAnim)BaseViewAnimation;
            mainScreenAnim.PlayToast();
        }

    }
    public void DailyRewardButton()
    {
        //Debug.Log("Daily Reward Button");
        DialogManager.Instance.ShowDialog(DialogIndex.DailyRewardDialog);
    }
    public void SpinView()
    {
        ///Debug.Log("View SPin Button");

        ViewManager.Instance.SwitchView(ViewIndex.CollectionView);
    }

}
