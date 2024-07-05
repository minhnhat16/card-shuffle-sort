using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Test class, nothing useful here
/// </summary>
public class ShrinkAndExpand : MonoBehaviour
{
    bool isExpanded;
    public Vector2 targetSize;
    Vector2 startSize;
    RectTransform selfRect;

    public float speed = 7;

    public Text targetText;

    private void Start() {
        selfRect = GetComponent<RectTransform>();
        startSize = selfRect.sizeDelta;
    }

    private void Update() {
        if (isExpanded) {
            selfRect.sizeDelta = Vector2.Lerp(selfRect.sizeDelta, targetSize, speed * Time.deltaTime);
        } else {
            selfRect.sizeDelta = Vector2.Lerp(selfRect.sizeDelta, startSize, speed * Time.deltaTime);
        }
    }

    public void SwitchState() {
        isExpanded = !isExpanded;
        targetText.text = isExpanded ? "Click to shrink" : "Click to expand";
    }
}
