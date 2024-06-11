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
        yield return new WaitForSeconds(0.5f);
        InitDataDone(() =>
        {
            gameManager.SetUpIngame();
            gameManager = GetComponentInChildren<GameManager>();
            gameManager.TrackLevelStart = 0;
            ZenSDK.instance.TrackLevelStart(gameManager.TrackLevelStart);
        });
        yield return new WaitForSeconds(1.5f);
        InitConfig();

    }
    private void InitDataDone(Action callback)
    {
        DataAPIController.instance.InitData(() =>
        {
            callback?.Invoke();
        });
    }
    IEnumerator SetUpUI(Action callback)
    {
        uiRoot.SetActive(true);
        yield return new WaitForSeconds(1f);
        LoadSceneManager.instance.LoadSceneByName("Buffer", () =>
        {
            Debug.Log("LoadSenceCallback");
            DayTimeController.instance.CheckNewDay();

            ZenSDK.instance.ShowAppOpen((isDone) =>
            {
                DialogManager.Instance.ShowDialog(DialogIndex.LableChooseDialog);
                ViewManager.Instance.SwitchView(ViewIndex.MainScreenView);
                SoundManager.instance.PlayMusic(SoundManager.Music.GamplayMusic);
                Debug.LogWarning("SHOW APP OPEN ON END LOADING");
                if (DayTimeController.instance.isNewDay)
                {
                    Debug.Log("isnewday now go to claim spin reward");
                    DataAPIController.instance.SetSpinData(false);
                }
                else
                {
                    Debug.Log("still in last day can't claim spin reward");
                    //DialogManager.Instance.ShowDialog(DialogIndex.LabelChooseDialog);
                }
            });

        });
        yield return null;
        callback?.Invoke();
    }
    private void SetupAfterInitConfig()
    {
        MainScreenViewParam param = new();
        param.totalGold = DataAPIController.instance.GetGold();


    }
    private void InitConfig()
    {
        ConfigFileManager.Instance.Init(() =>
        {
            StartCoroutine(SetUpUI(() =>
            {
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

