using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.WSA;

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
    private void Start()
    {
        lv_lb = GetComponentInChildren<Text>();
        currentLevel = DataAPIController.instance.GetPlayerLevel();
        SetLevelLable(currentLevel);

        currentExp = GetCurrentExp();
        record = ConfigFileManager.Instance.LevelConfig.GetAllRecord(); // change with config file 
        currentLevel = IngameController.instance.GetPlayerLevel();
        targetExp = record[currentLevel].Experience;

        fill.fillAmount = currentExp / targetExp;
        FillAmountToPercent(fill.fillAmount);
        //StartCoroutine(GetConfig());
    }
    private void Update()
    {

    }
    void FillAmountToPercent(float fillAmount)
    {
        fillAmount *= 100;
        percent.text = $"{Math.Round(fillAmount)}%";
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

    IEnumerator FillOverTime(float target, float duration)
    {
        //Debug.Log("FILL OVERTIME");
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
    //HACK: (DONE) CHECK CONDITIONAL FOR LEVEL UP BETWEEN THIS AND INGAMECONTROLLER
    private void LevelUp()
    {
        Debug.Log($"Level up!!!! {currentLevel }");
        LevelConfigRecord newLevel = record[currentLevel];

        currentLevel = newLevel.Id;

        SetLevelLable(currentLevel);
        IngameController.instance.SetPlayerLevel(currentLevel);
        targetExp = newLevel.Experience;

        IngameController.instance.Exp_Current = fill.fillAmount = currentExp = 0;
        // TODO: MAKE NEW DIALOG FOR CHOOSE CARD & CLAIMING COIN + COIN ANIM
        PickCardParam param = new();

        param.premium = newLevel.PremiumColor;
        param.free = newLevel.FreeColor;

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
        Debug.Log("Reset Fill" + fill.fillAmount + " pecent" + percent.text);
    }
}
