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
        StartCoroutine(InitCouroutine(callback));
    }

    public IEnumerator InitCouroutine(Action callback)
    {
        yield return new WaitUntil(() => DataAPIController.instance.isInitDone);
        string path = "Prefabs/UIPrefab/LevelItem";
        string iconPath = "Prefabs/UIPrefab/SelectionIcon";

        var cardColorData = DataAPIController.instance.GetAllCardColorType();
        bool initDone = false;
        for (int i = 0; i < cardColorData.Count; i++)
        {
            LevelItem newLevel = Instantiate(Resources.Load<LevelItem>(path), container);
            newLevel.CardType = (CardType)i;
            newLevel.ListCardColor = DataAPIController.instance.GetDataColorByType(newLevel.CardType).color;
            newLevel.Init();
            LevelItems.Add(newLevel);
            GameObject icon = (GameObject)Instantiate(Resources.Load(iconPath), selectionIconParent);
            yield return null;
            if (i == cardColorData.Count - 1) initDone = true;
        }
        //Debug.Log("for instantiate card done");
        yield return new WaitUntil(() => initDone);
        callback?.Invoke();
    }
}
