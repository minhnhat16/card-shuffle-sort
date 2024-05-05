using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dealer:MonoBehaviour
{
    public Slot dealSlot;
    public Image fillImg;
    public float fillOffset;
    public RectTransform dealerFill;

    public Transform fill;
    public Transform _anchorPoint;
    public IEnumerator Start()
    {
        yield return new WaitUntil(() => CameraMain.instance.main != null);
        //dealerFill.transform.position = CameraMain.instance.main.WorldToScreenPoint(transform.position + 1.5f* Vector3.down ) ;
        //fillImg.transform.position = fill.position;
        ScreenToWorld.Instance.SetWorldToCanvas(dealerFill);
    }
    public void Update()
    {
        int cardCout = dealSlot._cards.Count;
        if (cardCout !=0) return;
         fillImg.fillAmount = Mathf.Lerp(fillImg.fillAmount, 0, 5f * Time.deltaTime);
         if(fillImg.fillAmount < 0.01f)
        {
            fillImg.fillAmount = 0;
        }
    }
    public void UpdateFillPostion()
    {
        //TODO: IF CAMERA CHANGED , Change fill positon
        ScreenToWorld.Instance.SetWorldToCanvas(dealerFill);
        dealerFill.transform.SetPositionAndRotation(_anchorPoint.position, Quaternion.identity);    
    }
}