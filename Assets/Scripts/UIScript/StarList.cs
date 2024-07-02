using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;

public class StarList : MonoBehaviour
{
    [SerializeField] private List<StartRate> _stars = new List<StartRate>();
    [SerializeField] private Action callback;
    [HideInInspector]
    public UnityEvent<int> starEvent = new UnityEvent<int>();
    [HideInInspector]
    public UnityEvent<bool> rateEvent = new UnityEvent<bool>();
    private void OnEnable()
    {
        rateEvent =GetComponentInParent<RateDialog>().rateEvent;
        starEvent.AddListener(StarClickEvent);
    }
    private void OnDisable()
    {
        starEvent.RemoveListener(StarClickEvent);
    }
    private void Start()
    {
        //Init();
    }
   public void StarsListOff()
    {
        foreach (var star in _stars)
        {
            star.SetOnStar(false);
        }
    }
    private void StarClickEvent(int idStar)
    {
        //Debug.Log($"StarClickEvent {idStar}");
        foreach (var star in _stars)
        {
            //Debug.Log($"FOREACH START IN LISST {star.IDStar}");

            if (star.IDStar  <= idStar)
            {
                star.SetOnStar(true);
            }
            else
            {
                star.SetOnStar(false);
            }
        }
        rateEvent?.Invoke(true);
    }
    public void StarListConfirm( Action callback)
    {
        this.callback = callback;
        Debug.Log($"StarListConfirm {callback}");
        StartNextStar(0);
    }
    public void StartNextStar(int index )
    {
        if (index < _stars.Count ) {
            Debug.Log($"StartNextStar{index}");
            if (_stars[index].IsOn)
                _stars[index].ConfirmStarRate(() =>
                {
                    
                    index++;
                    if (index == 5)
                    {
                        Debug.Log("Start last star invoker");
                        callback?.Invoke();
                        return;
                    }
                    Debug.Log(index);
                    StartNextStar(index);
                });
            else
            {
                Debug.Log("Start next star invoker");
                callback?.Invoke();
            }
        }
        
    }
    public void StarsHide()
    {
        foreach(var star in _stars)
        {
            star.HideStarAnim();
        }
    }
    public void Init()
    {
        for(int i = 0; i < 5; i++) 
        { 

            var item = Instantiate((Resources.Load("Prefab/UIPrefab/Star", typeof(GameObject))), transform) as GameObject;
            if (item == null)
            {
                Debug.LogError(" item == null");
            }
            else
            {
                Debug.LogError(" item != null");

                _stars.Add(item.GetComponent<StartRate>());
                _stars[i].IDStar = i;
                _stars[i].IsOn = false;
                _stars[i].starEvent = this.starEvent;
            }
        }
    }
}
