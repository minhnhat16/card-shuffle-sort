using DanielLochner.Assets.SimpleScrollSnap;
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
    void SpawnToggle(int index)
    {
        Toggle toggle = TooglePool.Instance.pool.SpawnNonGravityWithIndex(index);
        toggle.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity); 
        toggle.transform.SetParent(scrollSnap.Pagination.transform);
        toggle.transform.SetPositionAndRotation(scrollSnap.Pagination.transform.position + new Vector3(toggleWidth * (scrollSnap.NumberOfPanels + 1), 0, 0), Quaternion.identity);
        toggle.group = toggleGroup;
    }
    {
        Debug.Log("Init dynamic content");
        int totalLevel = GameManager.instance.TotalLevel;
        for (int i = 0;i < totalLevel; i++)
        {
            Add(i);
        }
    }
    public void Add(int index)
    {
        // Pagination
        SpawnToggle(index);
        scrollSnap.Pagination.transform.position -= new Vector3(toggleWidth / 2f, 0, 0);
        // Panel
        //panelPrefab.GetComponent<Image>().color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        GameObject item = LevelItemPool.Instance.pool.SpawnNonGravityWithIndex(index).gameObject;
        scrollSnap.Add(item, index);
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
