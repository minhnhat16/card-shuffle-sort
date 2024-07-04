using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPanel : MonoBehaviour
{
    //[SerializeField] private GameObject prefab;
    //[SerializeField] private GameObject selectionIcon;
    [SerializeField] private LevelConfig config;
    [SerializeField] private ScrollSnapRect levelScrollSnap;
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
        string path = "Prefabs/UIPrefab/LevelItem";
        string iconPath = "Prefabs/UIPrefab/SelectionIcon";
        for (int i = 0; i < 9; i++)
        {
            LevelItem newLevel = Instantiate(Resources.Load<LevelItem>(path), container);
            LevelItems.Add(newLevel);
            Instantiate(Resources.Load(iconPath), selectionIconParent);
        }
    }

    public void InitCouroutine(Action callback)
    {
        var cardColorData = DataAPIController.instance.GetAllCardColorType();
        //levelScrollSnap.gameObject.SetActive(true);
        
        for (int i = 0; i < LevelItems.Count; i++)
        {
            var levelItem = LevelItems[i];
            levelItem.CardType = (CardType)i;
            var Count = DataAPIController.instance.GetCardDataCount((CardType)i) ;
            levelItem.CardCount = Count;
            levelItem.Init();
            if(i == LevelItems.Count - 1) callback?.Invoke();
        }
        //Debug.Log("for instantiate card done");
    }
}
