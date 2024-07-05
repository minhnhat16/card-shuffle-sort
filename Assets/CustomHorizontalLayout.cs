using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CustomHorizontalLayout : MonoBehaviour
{
    public float spacing = 10f;

    private void Start()
    {
        UpdateLayout();
    }

    private void OnValidate()
    {
        UpdateLayout();
    }

    public void UpdateLayout()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        float totalWidth = 0f;
        foreach (RectTransform child in rectTransform)
        {
            if (child.gameObject.activeSelf)
            {
                totalWidth += child.sizeDelta.x + spacing;
            }
        }

        totalWidth -= spacing; // Remove the last added spacing

        float startX = -totalWidth / 2f;

        foreach (RectTransform child in rectTransform)
        {
            if (child.gameObject.activeSelf)
            {
                float childWidth = child.sizeDelta.x;
                child.anchoredPosition = new Vector2(startX + childWidth / 2f, 0);
                startX += childWidth + spacing;
            }
        }
    }
}
