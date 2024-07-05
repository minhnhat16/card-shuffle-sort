using DanielLochner.Assets.SimpleScrollSnap;
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
        toggleWidth = (togglePrefab.transform as RectTransform).sizeDelta.x * (Screen.width / 2048f); ;
    }

    private void Start()
    {
        for (int i = 0; i < 9; i++)
        {
            Add(i);
        }
    }
    public void Add(int index)
    {
        // Pagination
        Toggle toggle = Instantiate(togglePrefab, scrollSnap.Pagination.transform.position + new Vector3(toggleWidth * (scrollSnap.NumberOfPanels + 1), 0, 0), Quaternion.identity, scrollSnap.Pagination.transform);
        toggle.group = toggleGroup;
        scrollSnap.Pagination.transform.position -= new Vector3(toggleWidth / 2f, 0, 0);
        // Panel
        //panelPrefab.GetComponent<Image>().color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        scrollSnap.Add(panelPrefab, index);
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
