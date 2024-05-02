using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ExperienceBar : MonoBehaviour
{
    [SerializeField] private Text lv_lb;
    [SerializeField] private Image fill;
    [SerializeField] private int currentLevel;
    [SerializeField] private int targetExp;
    [SerializeField] private List<LevelConfigRecord> record;

    [HideInInspector] public UnityEvent<int> onExpChanged = new();
    private void OnEnable()
    {
        onExpChanged = IngameController.instance.onExpChange == null ? null: IngameController.instance.onExpChange;
        onExpChanged.AddListener(ExpChanged);
    }
    private void Start()
    {
        currentLevel = DataAPIController.instance.GetPlayerLevel();
        fill.fillAmount = 0;
        record = ConfigFileManager.Instance.LevelConfig.GetAllRecord(); // change with config file 
        //targetExp = record[1].Experience;
    }
    private void ExpChanged(int exp)
    {
        currentLevel += exp;

        fill.fillAmount += (float)(exp) * (1/(float)targetExp);
        Debug.Log("Expchanged" + fill.fillAmount);

        if (currentLevel >= targetExp)
        {
            ResetFill();
            LevelUp();
            return;
        }
    }
    private void LevelUp()
    {
        Debug.Log("Level up!!!!");

    // TODO: MAKE NEW DIALOG FOR CHOOSE CARD & CLAIMING COIN + COIN ANIM

    }
    private void ResetFill()
    {
        fill.fillAmount = 0;
    }
}
