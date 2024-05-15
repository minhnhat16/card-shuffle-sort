using System.Collections;
using UnityEngine;
public class SlotCamera : MonoBehaviour
{
    public static SlotCamera instance;
    public bool isScalingCamera;
    private Camera s_Camera;
    public float height;
    public float width;
    public GameObject _obj;
    private const float baseAspect = 1;
    public float rate;
    [SerializeField] private float mul_Time = 5f;
    [SerializeField] private float timer ;
    [SerializeField] private float initialOrthographicSize;
    [SerializeField] private float targetOrthorgraphicSize;

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        DataTrigger.RegisterValueChange(DataPath.SLOTDICT + "new", (newSlot) =>
         {
             Debug.Log("Data Trigger slot value change");
             IngameController.instance.AllSlotCheckCamera();
         });
    }
    private void Start()
    {
        s_Camera = GetComponent<Camera>();
        GetCameraAspect();
        initialOrthographicSize = s_Camera.orthographicSize;
    }
    public void GetCamera()
    {
        s_Camera = _obj.GetComponent<Camera>();
    }
    public Camera GetCam()
    {
        GetCamera();
        return s_Camera;
    }
    public void GetCameraAspect()
    {
        height = s_Camera.orthographicSize * 2;
        width = height * s_Camera.aspect;
    }
    public float GetLeft()
    {
        return s_Camera.transform.position.x - width * 0.5f;
    }
    public float GetRight()
    {
        return s_Camera.transform.position.x + width * 0.5f;
    }
    public float GetTop()
    {
        return s_Camera.transform.position.y + height * 0.5f;
    }
    public float GetBottom()
    {
        return s_Camera.transform.position.y - height * 0.5f;
    }
  
    public void ScaleByTimeCamera()
    {
        StartCoroutine(ScaleCamera());
    }
    private IEnumerator ScaleCamera()
    {
        timer = 0f;
        Vector3 initialPosition = s_Camera.transform.position;
        Vector3 targetPosition = new Vector3(initialPosition.x, initialPosition.y + 0.25f, initialPosition.z);
        targetOrthorgraphicSize = initialOrthographicSize + 1f;

        while (timer < mul_Time)
        {
            isScalingCamera = true;

            float t = timer / mul_Time;
            // Lerp for camera size
            s_Camera.orthographicSize = Mathf.Lerp(initialOrthographicSize, targetOrthorgraphicSize, t);
            // Lerp for camera position
            s_Camera.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            timer += Time.deltaTime;

            //IngameController.instance.AllSlotCheckCamera();

            yield return null;
        }
        isScalingCamera = false;
        // Ensure both camera size and position are accurate at the end time
        s_Camera.orthographicSize = initialOrthographicSize = targetOrthorgraphicSize;
        s_Camera.transform.position =targetPosition;
        
    }

}
