using UnityEngine;
using UnityEngine.UI;

public class Dealer:MonoBehaviour
{
    public Slot dealSlot;
    public Transform fill;
    public Image fillImg;
    public float fillOffset;
    public  void Start()
    {
        fill.position = transform.position - new Vector3(0, 0, fillOffset);
        fillImg.transform.position = fill.position;
    }
    public void Update()
    {
        int cardCout = dealSlot._cards.Count;
        if (cardCout > 0) return;
        fillImg.fillAmount = Mathf.Lerp(fillImg.fillAmount, 0, 5f * Time.deltaTime);
         if(fillImg.fillAmount < 0.01f)
        {
            fillImg.fillAmount = 0;
        }
    }

}