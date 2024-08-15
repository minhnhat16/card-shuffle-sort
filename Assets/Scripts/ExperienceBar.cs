using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{

    [SerializeField] private int currentLevel;
    [SerializeField] private float targetExp;
    [SerializeField] private float currentExp;
    [SerializeField] private Text lv_lb;
    [SerializeField] private Text percent;
    [SerializeField] private Image fill;
    [SerializeField] private List<LevelConfigRecord> record;

    [HideInInspector] public UnityEvent<float> onExpChanged = new();
    private void OnEnable()
    {
        onExpChanged = IngameController.instance.onExpChange ?? null;
        onExpChanged.AddListener(ExpChanged);
    }
    private void OnDisable()
    {
        onExpChanged.RemoveAllListeners();
    }
    public void Init()
    {
        StartCoroutine(InitCouroutine());
    }
    IEnumerator InitCouroutine()
    {
        lv_lb = GetComponentInChildren<Text>();
        yield return new WaitUntil(() =>
            {
               return lv_lb != null;
            });
        yield return new WaitUntil(()=>DataAPIController.instance != null);
        currentLevel = DataAPIController.instance.GetPlayerLevel();
        SetLevelLable(currentLevel);
        currentExp = GetCurrentExp();
        yield return new WaitUntil(() => ConfigFileManager.Instance != null);
        record = ConfigFileManager.Instance.LevelConfig.GetAllRecord(); // change with config file 
        yield return new WaitUntil(predicate: () => IngameController.instance.gameObject.activeInHierarchy);
        currentLevel = IngameController.instance.GetPlayerLevel();
        targetExp = record[--currentLevel].Experience;
        fill.fillAmount = currentExp / targetExp;
        FillAmountToPercent(fill.fillAmount);
        //StartCoroutine(GetConfig());
    }
    void FillAmountToPercent(float fillAmount)
    {
        fillAmount *= 99;
        percent.text = $"{Math.Floor(fillAmount)}%";
    }
    //Get Current experience from ingame controller
    public float GetCurrentExp()
    {
        float currentExp = DataAPIController.instance.GetCurrentExp();
        //Debug.Log($"CURRENT EXP {currentExp}"); 
        return currentExp;
    }
    public void SetCurrentExp()
    {
        DataAPIController.instance.SetCurrentExp(currentExp, () =>
        {
            IngameController.instance.Exp_Current = currentExp;
        });
    }
    private bool CheckMaxLevel()
    {
        int crLever = IngameController.instance.GetPlayerLevel();
        int maxLevel = record.Count() -1;
        return crLever > maxLevel;
    }
    IEnumerator FillOverTime(float target, float duration)
    {
        //Debug.Log("FILL OVERTIME");
        bool isMaxLevel = CheckMaxLevel();
        if (isMaxLevel)
        {
            fill.fillAmount = 1;
            FillAmountToPercent(fill.fillAmount);
            yield return null;
        }
        float startFillAmount = fill.fillAmount;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fill.fillAmount = Mathf.Lerp(startFillAmount, target / targetExp, elapsed / duration);
            FillAmountToPercent(fill.fillAmount);
            yield return null;
        }
        fill.fillAmount = target / targetExp;
        if (currentExp >= targetExp)
        {
            ResetFill();
            LevelUp();
            yield return null;
        }
    }

    private void ExpChanged(float exp)
    {
        currentExp += exp;
        StartCoroutine(FillOverTime(currentExp, 0.5f));
        DataAPIController.instance.SetCurrentExp(currentExp, null);

    }
    public void SetLevelLable(int level)
    {
        //Debug.Log($"SET LEVEL LABLE {level}");

        lv_lb.text = level.ToString();
    }
    public int LevelRecordCheck(CardType type)
    {
        if (type == CardType.Default) return currentLevel;
        int level;
        level = Mathf.Abs((int)type*10 - currentLevel);
        return level;
    }
    //HACK: (DONE) CHECK CONDITIONAL FOR LEVEL UP BETWEEN THIS AND INGAMECONTROLLER
    private void LevelUp()
    {
        int level = LevelRecordCheck(IngameController.instance.CurrentCardType);
        LevelConfigRecord newLevel = record[--currentLevel];
        LevelConfigRecord newColor = record[level];
        currentLevel = newLevel.Id;

        Debug.LogError($"Level up!!!! {currentLevel }");    
        SetLevelLable(currentLevel);
        IngameController.instance.SetPlayerLevel(currentLevel);
        targetExp = newLevel.Experience;

        IngameController.instance.Exp_Current = fill.fillAmount = currentExp = 0;
        // TODO: MAKE NEW DIALOG FOR CHOOSE CARD & CLAIMING COIN + COIN ANIM
        PickCardParam param = new();


        param.premium = newColor.PremiumColor;
        param.free = newColor.FreeColor;

        DialogManager.Instance.ShowDialog(DialogIndex.PickCardDialog, param, () =>
        {
            ResetFill();
            Player.Instance.isAnimPlaying = true;
        });


    }
    private void ResetFill()
    {
        IngameController.instance.Exp_Current = fill.fillAmount = currentExp = 0;
        FillAmountToPercent(fill.fillAmount);
        //Debug.Log("Reset Fill" + fill.fillAmount + " pecent" + percent.text);
    }
}
