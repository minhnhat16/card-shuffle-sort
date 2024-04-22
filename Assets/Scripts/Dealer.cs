using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dealer:MonoBehaviour
{
    public Slot dealSlot;
    public Transform fill;
    public Image fillImg;
    public float fillOffset;
    public RectTransform dealerFill;
    public IEnumerator Start()
    {
        yield return new WaitUntil(() => CameraMain.instance.main != null);
        dealerFill.transform.position = CameraMain.instance.main.WorldToScreenPoint(transform.position + 1.5f* Vector3.down ) ;
        //fillImg.transform.position = fill.position;
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

}