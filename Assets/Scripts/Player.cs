using DG.Tweening;
using System.Collections;
using UnityEngine;

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
        yield return new WaitUntil(() => CameraMain.instance.main != null);
        //cam = CameraMain.instance.main;
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
        if (Input.touchCount <= 0) return;
        Touch touch = Input.GetTouch(0);
        Ray ray = cam.ScreenPointToRay(touch.position);
        if (!Physics.Raycast(ray, out var hit)) return;

        GameObject tObjct = hit.collider.gameObject;
        if (touch.phase == TouchPhase.Began)
        {
            Debug.Log("Touch began");   
            if (tObjct.transform.parent.TryGetComponent(out Slot s)) 
            {
                //Debug.Log($"Slot {s.gameObject} + slotID {s.ID}");  
                switch (s.status)
                {
                    
                    case SlotStatus.Active:
                        //Debug.Log("Clicked on Active Slot");
                        s.TapHandler();
                        break;
                    //case SlotStatus.Locked:
                    //    s.UnlockSlot();
                    //    break;
                }
            }
            //else if (tObjct.TryGetComponent(out DealButton btn))
            //{
            //    if (isAnimPlaying) return;
            //    if (!isDealBtnActive)
            //    {
            //        isDealBtnActive = true;
            //    }
            //    btn.HandleTap();
            //}
        }
    }
}
