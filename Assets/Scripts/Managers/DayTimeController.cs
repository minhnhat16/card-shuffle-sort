using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DayTimeController : MonoBehaviour
{
    public static DayTimeController instance;
    public bool isNewDay;
    public string lastCheckedTimeCard;
    public UnityEvent<bool> newDateEvent = new UnityEvent<bool>();

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
        newDateEvent.RemoveAllListeners();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frsame
    private void LateUpdate()
    {
    }
    public void CheckNewDay()
    {
        string lastDay = DataAPIController.instance.GetDayTimeData();
        DateTime last;
        if (DateTime.TryParse(lastDay, out last))
        {
            int compareResult = DateTime.Today.CompareTo(last);
            if (compareResult > 0)
            {
                Debug.Log("Last day is earlier than today.");
                isNewDay = true;
                DataAPIController.instance.SetDayTimeData(DateTime.Today.ToString());
            }
            else if (compareResult <= 0)
            {
                isNewDay = false;
                Debug.Log("Last day is the same as today.");
            }
        }
        else return;
    }
    public void NewDay(bool isNew)
    {
        isNewDay = isNew;
        if(isNew)
        {
            newDateEvent?.Invoke(true);
        }
    }
    //IEnumerator UpdateTime(DateTime targetTime, Text lable)
    //{
    //    Debug.Log("UPDATE TIME");
    //    while (true)
    //    {
    //        // Tính toán th?i gian còn l?i
    //        TimeSpan timeRemaining = targetTime - DateTime.Now;

    //        // N?u th?i gian ?ã h?t, d?ng b? ??m
    //        if (timeRemaining.TotalSeconds <= 0)
    //        {
    //            lable.text = "Card is full now";
    //            Debug.Log("Target time reached!");
    //            // Th?c hi?n các hành ??ng khác t?i ?ây n?u c?n
    //            isCountingTime = false;
    //            currentCardCounter = maxCardCounter;
    //            DataAPIController.instance.SetCurrrentCardPool(currentCardCounter, null);
    //            FillCardCounter(currentCardCounter, maxCardCounter);
    //            yield break;
    //        }

    //        //lb_timeCounter.text = $"500 cards in {minutes}:{seconds}";
    //        lb_timeCounter.text = "500 cards in " + string.Format("{0:00}:{1:00}", timeRemaining.Minutes, timeRemaining.Seconds);
    //        yield return null;
    //    }
    //}
}
