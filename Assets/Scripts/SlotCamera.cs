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
    public float rate;
    [SerializeField] private float mul_Time = 5f;
    [SerializeField] private float timer;
    [SerializeField] private float initialOrthographicSize;
    [SerializeField] private float targetOrthorgraphicSize;
    [SerializeField] private float addSize;

    public Vector3 targetPoint;
    public float scaleValue;
    public float Timer { get => timer; set => timer = value; }
    public float Mul_Time { get => mul_Time; set => mul_Time = value; }

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

        // Load saved values
        if (PlayerPrefs.HasKey("CameraOrthographicSize"))
        {
            initialOrthographicSize = s_Camera.orthographicSize = PlayerPrefs.GetFloat("CameraOrthographicSize");
        }
        else
        {
            initialOrthographicSize = s_Camera.orthographicSize;
        }

        if (PlayerPrefs.HasKey("CameraPositionX") && PlayerPrefs.HasKey("CameraPositionY"))
        {
            s_Camera.transform.position = new Vector3(
                PlayerPrefs.GetFloat("CameraPositionX"),
                PlayerPrefs.GetFloat("CameraPositionY"),
                s_Camera.transform.position.z
            );
        }
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

        isScalingCamera = false;

        // Ensure both camera size and position are accurate at the end time
        s_Camera.orthographicSize=  initialOrthographicSize = targetOrthorgraphicSize;
        s_Camera.transform.position = new Vector3(targetPoint.x, targetPoint.y, initialPosition.z);

        // Save values
        PlayerPrefs.SetFloat("CameraOrthographicSize", s_Camera.orthographicSize);
        PlayerPrefs.SetFloat("CameraPositionX", s_Camera.transform.position.x);
        PlayerPrefs.SetFloat("CameraPositionY", s_Camera.transform.position.y);
        PlayerPrefs.Save();
    }
}
