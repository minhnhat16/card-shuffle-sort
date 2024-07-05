using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class AutoSizeLayout : MonoBehaviour
{
    public bool isLoopUpdate; //Is need to update layout in void Update

    public bool dontTouchChildren = false; //Is need to move the children

    public enum States {
        VerticalTop,
        VerticalCenter,
        VerticalBottom,
        HorizontalLeft,
        HorizontalCenter,
        HorizontalRight
    }

    public States typeLayout; //Type of the layout

    public bool isResizeSelf = true; //Is need to resize self

    public bool isUseAdditionalPadding = false; //Is need to use additional padding. For example for Vertical type,
                                                //the additional will be top pad and bottom pad and vice versa

    public float topPad; //Top padding
    public float bottomPad; //Bottom padding
    public float leftPad; //Left padding
    public float rightPad; //Right padding

    public float spacing; //Spacing between objects

    public int repeatFrames = 2; //How many frames it should update layout after first update

    public bool isHaveMinSizeX; //Is need to set min size of layout
    public bool isHaveMinSizeY;
    public Vector2 minSize; //Min size of layout
    public bool isHaveMaxSizeX; //Is need to set max size of layout
    public bool isHaveMaxSizeY;
    public Vector2 maxSize; //Max size of layout

    float sizeTotal; // Total size of the object

    public RectTransform minSizeTargetRect; //If set then min size of layout will be the same as the size of target rect

    public bool isInverted; // By default - if you set a tag NotInLayout to a child object - it wont be calculated
                            // But if isInverted sets to true - it will calculate only children with the tag NotInLayout

    void Update() {
        if (isLoopUpdate) {
            UpdateLayout(false);
        }
    }

    /// <summary>
    /// Update layout method
    /// </summary>
    /// <param name="isRepeat">Is need to repeat update "repeatFrames" frames</param>
    /// <param name="isRecursive">Is need to launch this method in each child component</param>
    public void UpdateLayout(bool isRepeat = true, bool isRecursive = false) {
        UpdateAllRect(isRecursive);
        if (isRepeat) {
            if(updateRoutine != null) {
                StopCoroutine(updateRoutine);
            }
            if (gameObject.activeInHierarchy) {
                updateRoutine = StartCoroutine(UpdateRepeate(isRecursive));
            }
        }
    }

    void UpdateAllRect(bool isRecursive) {
        if (minSizeTargetRect != null) {
            minSize = minSizeTargetRect.rect.size;
        }
        switch (typeLayout) {
            case States.VerticalTop:
                sizeTotal = topPad;
                for (int i = 0; i < transform.childCount; i++) {
                    if ((isInverted ? transform.GetChild(i).tag == "NotInLayout" : transform.GetChild(i).tag != "NotInLayout")
                        && transform.GetChild(i).gameObject.activeSelf) {
                        var rect = transform.GetChild(i).GetComponent<RectTransform>();
                        if (isRecursive) {
                            if (rect.GetComponent<AutoSizeLayout>()) {
                                rect.GetComponent<AutoSizeLayout>().UpdateLayout(isRecursive: true);
                            }
                        }
                        if (!dontTouchChildren) {
                            rect.anchorMax = new Vector2(0.5f, 1);
                            rect.anchorMin = new Vector2(0.5f, 1);
                            rect.pivot = new Vector2(rect.pivot.x, 1);
                            rect.anchoredPosition = new Vector2((isUseAdditionalPadding ? leftPad - rightPad : rect.anchoredPosition.x),
                                -rect.sizeDelta.y * rect.localScale.y * (1 - rect.pivot.y) - sizeTotal);
                        }
                        sizeTotal += rect.sizeDelta.y * rect.localScale.y + spacing;
                    }
                }
                sizeTotal -= spacing;
                sizeTotal += bottomPad;
                if (isResizeSelf) {
                    GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x,
                        Mathf.Clamp(sizeTotal, isHaveMinSizeY ? minSize.y : float.MinValue, isHaveMaxSizeY ? maxSize.y : float.MaxValue));
                }
                break;
            case States.VerticalBottom:
                sizeTotal = bottomPad;
                for (int i = 0; i < transform.childCount; i++) {
                    if ((isInverted ? transform.GetChild(i).tag == "NotInLayout" : transform.GetChild(i).tag != "NotInLayout")
                        && transform.GetChild(i).gameObject.activeSelf) {
                        var rect = transform.GetChild(i).GetComponent<RectTransform>();
                        if (isRecursive) {
                            if (rect.GetComponent<AutoSizeLayout>()) {
                                rect.GetComponent<AutoSizeLayout>().UpdateLayout(isRecursive: true);
                            }
                        }
                        if (!dontTouchChildren) {
                            rect.anchorMax = new Vector2(0.5f, 0);
                            rect.anchorMin = new Vector2(0.5f, 0);
                            rect.pivot = new Vector2(rect.pivot.x, 0);
                            rect.anchoredPosition = new Vector2((isUseAdditionalPadding ? leftPad - rightPad : rect.anchoredPosition.x),
                                rect.sizeDelta.y * rect.localScale.y * (rect.pivot.y) + sizeTotal);
                        }
                        sizeTotal += rect.sizeDelta.y * rect.localScale.y + spacing;
                    }
                }
                sizeTotal -= spacing;
                sizeTotal += topPad;
                if (isResizeSelf) {
                    GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x,
                        Mathf.Clamp(sizeTotal, isHaveMinSizeY ? minSize.y : float.MinValue, isHaveMaxSizeY ? maxSize.y : float.MaxValue));
                }
                break;
            case States.HorizontalLeft:
                sizeTotal = leftPad;
                for (int i = 0; i < transform.childCount; i++) {
                    if ((isInverted ? transform.GetChild(i).tag == "NotInLayout" : transform.GetChild(i).tag != "NotInLayout")
                        && transform.GetChild(i).gameObject.activeSelf) {
                        var rect = transform.GetChild(i).GetComponent<RectTransform>();
                        if (isRecursive) {
                            if (rect.GetComponent<AutoSizeLayout>()) {
                                rect.GetComponent<AutoSizeLayout>().UpdateLayout(isRecursive: true);
                            }
                        }
                        if (!dontTouchChildren) {
                            rect.anchorMax = new Vector2(0f, 0.5f);
                            rect.anchorMin = new Vector2(0f, 0.5f);
                            rect.pivot = new Vector2(0, rect.pivot.y);
                            rect.anchoredPosition = new Vector2(rect.sizeDelta.x * rect.localScale.x * (rect.pivot.x) + sizeTotal,
                                (isUseAdditionalPadding ? bottomPad - topPad : rect.anchoredPosition.y));
                        }
                        sizeTotal += rect.sizeDelta.x * rect.localScale.x + spacing;
                    }
                }
                sizeTotal -= spacing;
                sizeTotal += rightPad;
                if (isResizeSelf) {
                    GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Clamp(sizeTotal, isHaveMinSizeX ? minSize.x : float.MinValue,
                        isHaveMaxSizeX ? maxSize.x : float.MaxValue), GetComponent<RectTransform>().sizeDelta.y);
                }
                break;
            case States.HorizontalRight:
                sizeTotal = rightPad;
                for (int i = 0; i < transform.childCount; i++) {
                    if ((isInverted ? transform.GetChild(i).tag == "NotInLayout" : transform.GetChild(i).tag != "NotInLayout")
                        && transform.GetChild(i).gameObject.activeSelf) {
                        var rect = transform.GetChild(i).GetComponent<RectTransform>();
                        if (isRecursive) {
                            if (rect.GetComponent<AutoSizeLayout>()) {
                                rect.GetComponent<AutoSizeLayout>().UpdateLayout(isRecursive: true);
                            }
                        }
                        if (!dontTouchChildren) {
                            rect.anchorMax = new Vector2(1f, 0.5f);
                            rect.anchorMin = new Vector2(1f, 0.5f);
                            rect.pivot = new Vector2(1, rect.pivot.y);
                            rect.anchoredPosition = new Vector2(-rect.sizeDelta.x * rect.localScale.x * (1 - rect.pivot.x) - sizeTotal,
                                (isUseAdditionalPadding ? bottomPad - topPad : rect.anchoredPosition.y));
                        }
                        sizeTotal += rect.sizeDelta.x * rect.localScale.x + spacing;
                    }
                }
                sizeTotal -= spacing;
                sizeTotal += leftPad;
                if (isResizeSelf) {
                    GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Clamp(sizeTotal, isHaveMinSizeX ? minSize.x : float.MinValue,
                        isHaveMaxSizeX ? maxSize.x : float.MaxValue), GetComponent<RectTransform>().sizeDelta.y);
                }
                break;
            case States.HorizontalCenter:
                float startSize = -leftPad;
                sizeTotal = leftPad;
                for (int i = 0; i < transform.childCount; i++) {
                    if ((isInverted ? transform.GetChild(i).tag == "NotInLayout" : transform.GetChild(i).tag != "NotInLayout")
                        && transform.GetChild(i).gameObject.activeSelf) {
                        var rect = transform.GetChild(i).GetComponent<RectTransform>();
                        if (isRecursive) {
                            if (rect.GetComponent<AutoSizeLayout>()) {
                                rect.GetComponent<AutoSizeLayout>().UpdateLayout(isRecursive: true);
                            }
                        }
                        if (!dontTouchChildren) {
                            rect.anchorMax = new Vector2(0f, 0.5f);
                            rect.anchorMin = new Vector2(0f, 0.5f);
                            rect.pivot = new Vector2(0.5f, rect.pivot.y);
                        }
                        startSize += rect.sizeDelta.x * rect.localScale.x + spacing;
                    }
                }
                startSize -= spacing;
                startSize += rightPad;
                var rectSelf = GetComponent<RectTransform>().rect;
                sizeTotal = rectSelf.width / 2 - startSize / 2;
                for (int i = 0; i < transform.childCount; i++) {
                    if ((isInverted ? transform.GetChild(i).tag == "NotInLayout" : transform.GetChild(i).tag != "NotInLayout")
                        && transform.GetChild(i).gameObject.activeSelf) {
                        var rect = transform.GetChild(i).GetComponent<RectTransform>();
                        if (!dontTouchChildren) {
                            rect.anchoredPosition = new Vector2(rect.sizeDelta.x * rect.localScale.x * (1 - rect.pivot.x) + sizeTotal,
                                 (isUseAdditionalPadding ? bottomPad - topPad : rect.anchoredPosition.y));
                        }
                        sizeTotal += rect.sizeDelta.x * rect.localScale.x + spacing;
                    }
                }
                sizeTotal -= spacing;
                sizeTotal += rightPad;
                if (isResizeSelf) {
                    GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Clamp(sizeTotal, isHaveMinSizeX ? minSize.x : float.MinValue,
                        isHaveMaxSizeX ? maxSize.x : float.MaxValue), GetComponent<RectTransform>().sizeDelta.y);
                }
                break;
            case States.VerticalCenter:
                float startSizeVertical = -topPad;
                sizeTotal = topPad;
                for (int i = 0; i < transform.childCount; i++) {
                    if ((isInverted ? transform.GetChild(i).tag == "NotInLayout" : transform.GetChild(i).tag != "NotInLayout")
                        && transform.GetChild(i).gameObject.activeSelf) {
                        var rect = transform.GetChild(i).GetComponent<RectTransform>();
                        if (isRecursive) {
                            if (rect.GetComponent<AutoSizeLayout>()) {
                                rect.GetComponent<AutoSizeLayout>().UpdateLayout(isRecursive: true);
                            }
                        }
                        if (!dontTouchChildren) {
                            rect.anchorMax = new Vector2(0.5f, 1f);
                            rect.anchorMin = new Vector2(0.5f, 1f);
                            rect.pivot = new Vector2(rect.pivot.x, 0.5f);
                        }
                        startSizeVertical += rect.sizeDelta.y * rect.localScale.y + spacing;
                    }
                }
                startSizeVertical -= spacing;
                startSizeVertical += bottomPad;
                var rectSelfVertical = GetComponent<RectTransform>().rect;
                sizeTotal = rectSelfVertical.height / 2 - startSizeVertical / 2;
                for (int i = 0; i < transform.childCount; i++) {
                    if ((isInverted ? transform.GetChild(i).tag == "NotInLayout" : transform.GetChild(i).tag != "NotInLayout")
                        && transform.GetChild(i).gameObject.activeSelf &&
                        transform.GetChild(i).tag != "InLayoutButNotIncluded") {
                        var rect = transform.GetChild(i).GetComponent<RectTransform>();
                        if (!dontTouchChildren) {
                            rect.anchoredPosition = new Vector2((isUseAdditionalPadding ? leftPad - rightPad : rect.anchoredPosition.x),
                                -rect.sizeDelta.y * rect.localScale.y * (1 - rect.pivot.y) - sizeTotal);
                        }
                        sizeTotal += rect.sizeDelta.y * rect.localScale.y + spacing;
                    }
                }
                sizeTotal -= spacing;
                sizeTotal += bottomPad;
                if (isResizeSelf) {
                    GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x,
                        Mathf.Clamp(sizeTotal, isHaveMinSizeY ? minSize.y : float.MinValue, isHaveMaxSizeY ? maxSize.y : float.MaxValue));
                }
                break;
        } 
    }

    Coroutine updateRoutine;
    IEnumerator UpdateRepeate(bool isRecursive) {
        for(int i = 0; i < repeatFrames; i++) {
            yield return new WaitForEndOfFrame();
            UpdateAllRect(isRecursive);
        }
    }
}
