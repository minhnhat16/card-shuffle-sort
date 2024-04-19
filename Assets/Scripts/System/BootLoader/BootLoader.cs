using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
public class BootLoader : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject uiRoot;
    [SerializeField] private UIRootControlScale uiRootControl;
    IEnumerator Start()
    {
        DontDestroyOnLoad(this);
        yield return new WaitForSeconds(0.1f);
        InitDataDone(() =>
        {

        });

    }
    private void InitDataDone(Action callback)
    {
        DataAPIController.instance.InitData(() =>
        {
            InitConfig();
            callback?.Invoke(); 
        });
    }
    IEnumerator SetUpUI(Action callback)
    {
        uiRoot.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        LoadSceneManager.instance.LoadSceneByName("Buffer", () =>
        {
            Debug.Log("LoadSenceCallback");
            DayTimeController.instance.CheckNewDay();

            ZenSDK.instance.ShowAppOpen((isDone) =>
            {
                SoundManager.Instance.PlayMusic(SoundManager.Music.GamplayMusic);
                Debug.LogWarning("SHOW APP OPEN ON END LOADING");
                if (DayTimeController.instance.isNewDay)
                {
                    Debug.Log("isnewday now go to claim spin reward");
                    ViewManager.Instance.SwitchView(ViewIndex.SpinView);
                }
                else
                {
                    Debug.Log("still in last day can't claim spin reward");
                    DialogManager.Instance.ShowDialog(DialogIndex.LabelChooseDialog);
                }
            });

        });
        yield return null;
        callback?.Invoke();
    }
    private void SetupAfterInitConfig()
    {
        SkinLibControl.Instance.InitFruitSkin();
        MainScreenViewParam param = new();
        param.totalGold = DataAPIController.instance.GetGold();
        

    }
    private void InitConfig()
    {
        ConfigFileManager.Instance.Init(() =>
        {
            DOTween.SetTweensCapacity(1000, 50);
            gameManager = GetComponentInChildren<GameManager>();
            gameManager.TrackLevelStart = 0;
            ZenSDK.instance.TrackLevelStart(gameManager.TrackLevelStart);
            gameManager.SetupGameManager();
            StartCoroutine(SetUpUI(() => {
                SetupAfterInitConfig();
                }));
        }); ;
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0;
        }
        else
            Time.timeScale = 1;
    }
    private void OnApplicationFocus(bool focus)
    {
        Time.timeScale = 1;
    }
}

