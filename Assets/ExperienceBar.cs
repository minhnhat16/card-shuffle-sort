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

    [HideInInspector] public UnityEvent<int> onExpChanged = new();
    private void OnEnable()
    {
        onExpChanged = IngameController.instance.onExpChange ==null? null: IngameController.instance.onExpChange;
        onExpChanged?.AddListener(ExpChanged);
    }
    private void Start()
    {
        currentLevel = DataAPIController.instance.GetPlayerLevel();
        targetExp = 100; // change with config file 
    }
    private void ExpChanged(int exp)
    {
        float fillPercent = exp / targetExp;
        float crFill = fill.fillAmount;
        do
        {
            crFill += Time.deltaTime;
        } while (crFill < fillPercent);

        if(fillPercent == 1)
        {
            ResetFill();
            LevelUp();
            return;
        }
    }
    private void LevelUp()
    {
        Debug.Log("Level up!!!!");
    }
    private void ResetFill()
    {
        fill.fillAmount = 0;
    }
}
