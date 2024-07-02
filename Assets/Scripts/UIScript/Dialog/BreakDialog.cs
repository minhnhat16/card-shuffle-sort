using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakDialog : BaseDialog
{
    private float timeRemaining = 30f;  // 30 seconds
    private int reward;
    private Currency currency;
    public Text timeCounter;
    public Text lb_reward;

    public override void OnStartShowDialog()
    {
        base.OnStartShowDialog();
        reward = ZenSDK.instance.GetConfigInt("coffeeReward", 1500);
        ZenSDK.instance.ShowVideoReward((isWatch) =>
        {
            if (isWatch) timeRemaining -= 10;
        });
    }
    public override void OnEndShowDialog()
    {
        base.OnEndShowDialog();
        if (timeCounter == null)
        {
            //Debug.LogError("No UI Text component assigned.");
            return;
        }
        StartCoroutine(StartCountdown());
    }


    private IEnumerator StartCountdown()
    {
        while (timeRemaining > 0)
        {
            UpdateTimerDisplay(timeRemaining);
            //yield return new WaitForSeconds(1f);
            timeRemaining--;
            yield return new WaitForSeconds(1f);
        }
        
        UpdateTimerDisplay(0);  // Ensure display shows 00:00 when finished
        if(timeRemaining <= 0)
        {
            DialogManager.Instance.HideDialog(dialogIndex, () =>
            {

                DataAPIController.instance.AddGold(reward);
                timeRemaining = 30f;
            });
        }
    }
    private void Reset()
    {
        timeRemaining = 30f;
    }
    private void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        string timeText = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timeCounter != null)
        {
            timeCounter.text = timeText;
        }
        if (timeCounter != null)
        {
            timeCounter.text = timeText;
        }
    }
}
