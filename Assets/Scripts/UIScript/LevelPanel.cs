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
    [SerializeField] private Transform container;

    [SerializeField]
    private Transform selectionIconParent;

    public List<LevelItem> LevelItems { get => _levelItems; set => _levelItems = value; }

    // Start is called before the first frame update

    public void Init(Action callback)
    {
       InitCouroutine(callback);
    }
    public void InitLevelItem()
    {
        for (int i = 0; i < LevelItemPool.Instance.total; i++)
        {
            var item = LevelItemPool.Instance.pool.list[i];
            
            LevelItems.Add(item);
            item.gameObject.SetActive(true);
        }
    }

    public void InitCouroutine(Action callback)
    {
        var cardColorData = DataAPIController.instance.GetAllCardColorType();
        //levelScrollSnap.gameObject.SetActive(true);
        
        for (int i = 0; i < LevelItems.Count; i++)
        {
            var levelItem = LevelItems[i];
            levelItem.CardType = (CardType)i    ;
            var Count = DataAPIController.instance.GetCardDataCount((CardType)i) ;
            levelItem.CardCount = Count;
            levelItem.Init();
            if(i == LevelItems.Count - 1) callback?.Invoke();
        }
        //Debug.Log("for instantiate card done");
    }
    public void IsScrollRectActive(bool isActive)
    {
        scrollRect.enabled = isActive;
    }
}
