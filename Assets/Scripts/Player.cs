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

    [SerializeField] private Camera cam;
    public bool isDealBtnActive;

    private void Awake()
    {
        Instance = this;
    }
    IEnumerator Start()
    {
        yield return new WaitUntil(() => CameraMain.instance.main != null);
        cam = CameraMain.instance.main;
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
        //Debug.Log("Touch count > 0");
        Touch touch = Input.GetTouch(0);

        Vector3 touchPosition = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, cam.nearClipPlane));

        // Cast a 2D ray from the touch position
        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
        if (!hit) return;

        GameObject touched = hit.collider.gameObject;

        //Debug.Log($"GAME OBJECT HAS BEEN TOUCHED {touched}");
        if (touch.phase == TouchPhase.Began)
        {
            if (touched.transform.parent != null && touched.transform.parent.TryGetComponent(out Slot slot))
            {
                switch (slot.status)
                {
                    case SlotStatus.Active:
                        //Debug.Log("Clicked on Active Slot");
                        slot.TapHandler();
                        break;
                    case SlotStatus.Locked:
                        slot.UnlockSlot();
                        break;
                }
            }
            else if (touched.TryGetComponent(out DealButton btn))
            {
                if (isAnimPlaying) return;
                if (!isDealBtnActive)
                {
                    isDealBtnActive = true;
                }
                btn.HandleTap();
            }
        }
    }
}
