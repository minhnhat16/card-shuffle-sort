using DG.Tweening;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public Slot fromSlot;
    public Slot toSlot;

    public float duration;
    public float delay;
    public float height;
    public Ease ease;

    public int totalGold;
    public int totalGem;
    public int playerLevel; //Add data to data model

    public float timeDisableCard;
    public bool isAnimPlaying;
    public float cardPositionOffsetY = 0.075f;
    public float cardPositionOffsetZ = -0.075f;

    [SerializeField] private Camera cam;
    public bool isDealBtnActive;

   

    private void Awake()
    {
        Instance = this;
    }
    IEnumerator Start()
    {
        yield return new WaitUntil(() => SlotCamera.Instance.S_Camera != null);
        cam = SlotCamera.Instance.GetCam();
    }
    private void Update()
    {
        if (isDealBtnActive) return;
        TouchHandle();
    }
    
    public bool IsFromSlotNull()
    {
        Debug.Log("IsFromSlotNull");

        if (fromSlot == null)
        {
            Debug.Log("NULL");
            return true;
        }
        return false;
    }
    public void TouchHandle()
    {
        if (Input.touchCount <= 0 ||GameManager.instance.IsNewPlayer) return;
        Touch touch = Input.GetTouch(0);
        Ray ray = cam.ScreenPointToRay(touch.position);
        if (!Physics.Raycast(ray, out var hit)) return;

        GameObject tObjct = hit.collider.gameObject;
       
        if (touch.phase == TouchPhase.Began )
        {   
            //Debug.Log("Touch began");   
            if (tObjct.transform.parent.TryGetComponent(out Slot s)) 
            {
                //Debug.Log($"Slot {s.gameObject} + slotID {s.ID}");  
                switch (s.status)
                {
                    
                    case SlotStatus.Active:
                        //Debug.Log("Clicked on Active Slot");
                        s.TapHandler();
                        break;
                }
            }
         
        }
    }

    public void PlayerTouchTutorial(TutorialStep currentStep,Action callback)
    {
        if (Input.touchCount <= 0) return;

        Touch touch = Input.GetTouch(0);
        Ray ray = cam.ScreenPointToRay(touch.position);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (touch.phase == TouchPhase.Began)
        {

            if(currentStep.collider == hit.collider)
            {
                Debug.Log("this is true collider");
                currentStep.PlayerHit(callback);
            }
        }
        
    }
}
