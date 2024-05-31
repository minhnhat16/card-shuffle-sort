using UnityEngine;
using UnityEngine.UI;

public class MainScreenView : BaseView
{
    //public int totalGold;
    //public TextMeshProUGUI gold_lb;
    [SerializeField] private Button playBtn;
    [SerializeField] ScrollSnapRect levelScroll;
    [SerializeField] private LevelPanel levelPanel;
    private void OnEnable()
    {
        playBtn.onClick.AddListener(OnPlayButton);
    }

    private void OnDisable()
    {
        playBtn.onClick.RemoveListener(OnPlayButton);
    }

    public override void Setup(ViewParam viewParam)
    {
        base.Setup(viewParam);

    }
    public override void OnInit()
    {
        playBtn.interactable = true;
        Debug.Log("Init main screen" +
            " Scroll Snapp Init Done");
        if (levelPanel.LevelItems.Count > 0) return;
        levelPanel.Init(() =>
        {
            Debug.Log("Sever Scroll Snapp Init Done");
            levelScroll.gameObject.SetActive(true);
        });
    }
    public override void OnStartShowView()
    {
        base.OnStartShowView();
        OnInit();
    }
    private void OnPlayButton()
    {
        Debug.Log("OnPlayButton");
        playBtn.interactable = false;
        int levelLoad = levelScroll.CurrentPage;

        Debug.Log("Current card type" + levelLoad);

        if (levelPanel.LevelItems[levelLoad].CheckUnlock())
        {
            DataAPIController.instance.SetCurrentCardType((CardType)levelLoad, () =>
            {
                DialogManager.Instance.HideAllDialog();
                IngameController.instance.Init();

            });
            LoadSceneManager.instance.LoadSceneByName("Ingame", () =>
            {
                ViewManager.Instance.SwitchView(ViewIndex.GamePlayView, null, () =>
                {
                });
            });
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
        Debug.Log("Daily Reward Button");
        DialogManager.Instance.ShowDialog(DialogIndex.DailyRewardDialog);
    }
    public void SpinView()
    {
        Debug.Log("View SPin Button");

        ViewManager.Instance.SwitchView(ViewIndex.CollectionView);
    }

}
