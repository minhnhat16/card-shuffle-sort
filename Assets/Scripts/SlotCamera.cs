using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotCamera : MonoBehaviour
{
    public static SlotCamera instance;
    public bool isScalingCamera;
    private Camera s_Camera;
    public float height;
    public float width;
    public GameObject _obj;
    public float rate;
    [SerializeField] private float mul_Time = 5f;
    [SerializeField] private float timer;
    [SerializeField] private float initialOrthographicSize;
    [SerializeField] private float targetOrthorgraphicSize;
    [SerializeField] private float addSize;

    public Vector3 targetPoint;
    public int mulCount = 0;
    [SerializeField] private  List<float> scaleValue;
    public float Timer { get => timer; set => timer = value; }
    public float Mul_Time { get => mul_Time; set => mul_Time = value; }
    public List<float> ScaleValue { get => scaleValue; set => scaleValue = value; }

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
    }
    public void Init()
    {
        StartCoroutine(InitCameraCoroutine());
    }
    IEnumerator InitCameraCoroutine()
    {
        yield return new WaitUntil(() => DataAPIController.instance.GetCameraData() !=null);
        Debug.Log("InitCamera");
        SlotCameraData newData = DataAPIController.instance.GetCameraData();
        mulCount = newData.scaleTime;
        initialOrthographicSize = s_Camera.orthographicSize = newData.OrthographicSize;
        s_Camera.transform.position = new Vector3(newData.positionX,newData.positionY,s_Camera.transform.position.z);
        GetCameraAspect();

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
        mulCount++;
        Vector3 initialPosition = s_Camera.transform.position;
        targetOrthorgraphicSize = initialOrthographicSize + addSize;

        while (timer < mul_Time)
        {
            isScalingCamera = true;

            float t = timer / mul_Time;

            // Lerp for camera size
            s_Camera.orthographicSize = Mathf.Lerp(initialOrthographicSize, targetOrthorgraphicSize, t);

            // Calculate the new position to focus on the target point
            Vector3 newPosition = Vector3.Lerp(initialPosition, new Vector3(targetPoint.x, targetPoint.y, initialPosition.z), t);
            s_Camera.transform.position = newPosition;

            timer += Time.deltaTime;

            yield return null;
        }
        //IngameController.instance.ReloadAllSlotButton();
        isScalingCamera = false;
        GetCameraAspect();
        // Ensure both camera size and position are accurate at the end time
        s_Camera.orthographicSize=  initialOrthographicSize = targetOrthorgraphicSize;
        s_Camera.transform.position = new Vector3(targetPoint.x, targetPoint.y, initialPosition.z);
        // Save values
        float x = s_Camera.transform.position.x;
        float y = s_Camera.transform.position.y;
        float orthographicSize = s_Camera.orthographicSize;

        DataAPIController.instance.SetCameraData(x, y, orthographicSize, mulCount, null);
     
    }
}
