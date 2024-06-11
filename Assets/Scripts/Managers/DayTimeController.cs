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
    private void Start()
    {
    }
    // Update is called once per frsame
    public  IEnumerator InitCouroutine()
    {
        yield return new WaitUntil(()=> DataAPIController.instance.isInitDone);
        CheckNewDay();
    }
    public void CheckNewDay()
    {
        DateTime now = DateTime.Now;
        DateTime last = DataAPIController.instance.GetTimeClaimItem();

        // Check if 'last' is a valid DateTime
        if (last != DateTime.MinValue)
        {
            // Calculate the time difference
            TimeSpan timeDifference = now - last;

            // Check if the difference is greater than 24 hours
            if (timeDifference.TotalHours > 24)
            {
                Debug.Log("More than 24 hours have passed since the last claim.");
                isNewDay = true;
                DataAPIController.instance.SetIsClaimTodayData(!isNewDay);
                //DataAPIController.instance.SetDayTimeData(now.ToString());
                NewDay(true);
            }
            else
            {
                Debug.Log("Less than 24 hours have passed since the last claim.");
                isNewDay = false;
            }
        }
        else
        {
            Debug.Log("Invalid last claim time.");
        }
    }

    public void NewDay(bool isNew)
    {
        isNewDay = isNew;
        if(isNew)
        {
            newDateEvent?.Invoke(true);
        }
    }

}
