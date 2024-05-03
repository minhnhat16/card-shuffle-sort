using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

public class ExperienceBar : MonoBehaviour
{
    [SerializeField] private Text lv_lb;
    [SerializeField] private Image fill;
    [SerializeField] private int currentLevel;
    [SerializeField] private float targetExp;
    [SerializeField] private float currentExp;
    [SerializeField] private List<LevelConfigRecord> record;

    [HideInInspector] public UnityEvent<int> onExpChanged = new();
    private void OnEnable()
    {
        onExpChanged = IngameController.instance.onExpChange /*== null ? null: IngameController.instance.onExpChange*/;
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
        fill.fillAmount = GetCurrentExp();
        record = ConfigFileManager.Instance.LevelConfig.GetAllRecord(); // change with config file 
        currentLevel = IngameController.instance.GetPlayerLevel();
        SetLevelLable(currentLevel);
        targetExp = record[currentLevel].Experience;
        //StartCoroutine(GetConfig());
    }
    //Get Current experience from ingame controller
    public float GetCurrentExp() 
    {
        float currentExp = IngameController.instance.Exp_Current;
        return IngameController.instance.Exp_Current;
    }
    public void SetCurrentExp()
    {
         IngameController.instance.Exp_Current = currentExp ;

    }


    private void ExpChanged(int exp)
    {
        currentExp += exp;

        fill.fillAmount = (float)(currentExp/ targetExp);
        Debug.Log("Expchanged" + fill.fillAmount);

        if (currentExp >= targetExp)
        {
            ResetFill();
            LevelUp();
            return;
        }
    }
    public void SetLevelLable(int level)
    {
        Debug.Log($"SET LEVEL LABLE {level}");
       
        lv_lb.text = level.ToString();
    }
    //HACK: CHECK CONDITIONAL FOR LEVEL UP BETWEEN THIS AND INGAMECONTROLLER
    private void LevelUp()
    {
        Debug.Log($"Level up!!!! {currentLevel }");
        LevelConfigRecord newLevel = record[currentLevel/* + 1*/];

        currentLevel = newLevel.Id;
        SetLevelLable(currentLevel);
        IngameController.instance.SetPlayerLevel(currentLevel);

        targetExp = newLevel.Experience;
    // TODO: MAKE NEW DIALOG FOR CHOOSE CARD & CLAIMING COIN + COIN ANIM

    }
    private void ResetFill()
    {
        fill.fillAmount = 0;
        currentExp = 0;
    }
}
