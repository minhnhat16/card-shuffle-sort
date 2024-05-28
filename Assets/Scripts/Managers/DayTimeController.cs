using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
}
