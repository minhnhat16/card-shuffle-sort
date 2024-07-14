using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;

public class DynamicContent : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private Toggle togglePrefab;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private SimpleScrollSnap scrollSnap;

    private float toggleWidth;
    #endregion

    #region Methods
    private void Awake()
    {
        toggleWidth = (togglePrefab.transform as RectTransform).sizeDelta.x * (Screen.width / 720f); ;
        Debug.Log("toggle width" + toggleWidth);
    }
    public void Init()
    {
        Debug.Log("Init dynamic content");
        int totalLevel = DataAPIController.instance.GetAllCardColorType().Count;
        for (int i = 0;i < totalLevel; i++)
        {
            Add(i);
        }
    }
    public void Add(int index)
    {
        //Debug.Log("init level item with index" + index);
        LevelItem item = LevelItemPool.Instance.pool.SpawnNonGravityWithIndex(index);
        item.CardType = (CardType)index;
        item.CardCount = DataAPIController.instance.GetCardDataCount((CardType)index);
        item.Init();
        scrollSnap.Add(item.gameObject, index);
    }
    public int GetCenterPageIndex()
    {
        return scrollSnap.CenteredPanel;
    }
    public void AddToFront()
    {
        Add(0);
    }
    public void AddToBack()
    {
        Add(scrollSnap.NumberOfPanels);
    }

    public void Remove(int index)
    {
        if (scrollSnap.NumberOfPanels > 0)
        {
            // Pagination
            DestroyImmediate(scrollSnap.Pagination.transform.GetChild(scrollSnap.NumberOfPanels - 1).gameObject);
            scrollSnap.Pagination.transform.position += new Vector3(toggleWidth / 2f, 0, 0);

            // Panel
            scrollSnap.Remove(index);
        }
    }

    public void RemoveFromFront()
    {
        Remove(0);
    }
    public void RemoveFromBack()
    {
        if (scrollSnap.NumberOfPanels > 0)
        {
            Remove(scrollSnap.NumberOfPanels - 1);
        }
        else
        {
            Remove(0);
        }
    }
    #endregion
}
