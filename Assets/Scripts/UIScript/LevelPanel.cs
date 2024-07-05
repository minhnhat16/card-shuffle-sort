using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour
{
    //[SerializeField] private GameObject prefab;
    //[SerializeField] private GameObject selectionIcon;
    [SerializeField] private LevelConfig config;
    [SerializeField] private ScrollSnapRect levelScrollSnap;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] List<LevelItem> _levelItems;
    [SerializeField] private GameObject levelItemContainer;
    [SerializeField] private Transform iconContainer;
    [SerializeField] 
    private DynamicContent content;
    [SerializeField]
    private Transform selectionIconParent;
  
    public void Init(Action callback)
    {
        Debug.Log("for init card done");
        content = GetComponent<DynamicContent>();   
        InitCouroutine(callback);
    }
    public LevelItem GetLeveItem(int index)
    {
        LevelItem item = _levelItems[index];
        if(item == null) return null;
        return item;  
    }
    public void InitCouroutine(Action callback)
    {
        var cardColorData = DataAPIController.instance.GetAllCardColorType();
        //LevelItem levelItem;
        //for (int i = 0; i <9; i++)
        //{
        //    levelItem = _levelItems[i];
        //    levelItem.CardType = (CardType)i    ;
        //    var Count = DataAPIController.instance.GetCardDataCount((CardType)i) ;
        //    levelItem.CardCount = Count;
        //    levelItem.Init();
        //    levelItem.gameObject.SetActive(true);
        //    if (i == 9 - 1) {
        //        Debug.Log("for instantiate card done");
        //        callback?.Invoke();
        //    };
        //}

    }
    public void IsScrollRectActive(bool isActive)
    {
        scrollRect.enabled = isActive;
        levelItemContainer.SetActive(true);
    }
}
