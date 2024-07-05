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
    private Transform selectionIconParent;

    public List<LevelItem> LevelItems { get => _levelItems; set => _levelItems = value; }

    //Start is called before the first frame update
    //private IEnumerator Start()
    //{
    //    bool initDone = false;
    //   InitCouroutine(() =>
    //    {
    //        initDone = true;
    //    };
    //    yield return new WaitUntil(() => initDone);
    //    yield return null;
    //    //foreach (var levelItem in LevelItems)
    //    //{
    //    //    yield return new WaitForSeconds(0.1f);
    //    //    levelItem.gameObject.SetActive(true);
    //    //}
    //}
    public void Init(Action callback)
    {
      InitCouroutine(callback);
    }
    public void InitLevelItem()
    {
        LevelItem a;
        for (int i = 0; i < LevelItemPool.Instance.total; i++)
        {
            a =  LevelItemPool.Instance.pool.list[i];
            LevelItems.Add(a);
        }
    }

    public void InitCouroutine(Action callback)
    {
        var cardColorData = DataAPIController.instance.GetAllCardColorType();
        LevelItem levelItem;
        for (int i = 0; i < LevelItems.Count; i++)
        {
            levelItem = LevelItems[i];
            levelItem.CardType = (CardType)i    ;
            var Count = DataAPIController.instance.GetCardDataCount((CardType)i) ;
            levelItem.CardCount = Count;
            levelItem.Init();
            levelItem.gameObject.SetActive(true);
            if(i == LevelItems.Count - 1) callback?.Invoke();
        }
        //Debug.Log("for instantiate card done");
    }
    public void IsScrollRectActive(bool isActive)
    {
        scrollRect.enabled = isActive;
        levelItemContainer.SetActive(true);
    }
}
