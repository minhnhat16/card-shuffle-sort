using UnityEngine;

public static class CanvasPositioningExtensions
{
    public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null, bool useNormalizeViewPort = false)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }

        var viewportPosition = camera.WorldToViewportPoint(worldPosition);

        if (useNormalizeViewPort)
        {
            Rect normalizedViewPort = camera.rect;
            viewportPosition.x = viewportPosition.x * normalizedViewPort.width + normalizedViewPort.x;
            viewportPosition.y = viewportPosition.y * normalizedViewPort.height + normalizedViewPort.y;
        }

        return canvas.ViewportToCanvasPosition(viewportPosition);
    }
    public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        var viewportPosition = camera.WorldToViewportPoint(worldPosition);
        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition)
    {
        var viewportPosition = new Vector3(screenPosition.x / Screen.width,
                                           screenPosition.y / Screen.height,
                                           0);
        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition)
    {
        var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
        var canvasRect = canvas.GetComponent<RectTransform>();
        var scale = canvasRect.sizeDelta;
        return Vector3.Scale(centerBasedViewPortPosition, scale);
    }
}
public class ScreenToWorld : MonoBehaviour
{
    public static ScreenToWorld Instance;
    public Camera m_UICamera;
    public Camera m_WCamera;
    public RectTransform m_Image;
    public RectTransform m_Parent;
    public Canvas m_Canvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    public void SetWorldToCanvas(RectTransform gameObject)
    {
        m_UICamera = CameraMain.instance.GetCam();
        if (gameObject == null) return;
        Vector3 ViewportPosition = CanvasPositioningExtensions.WorldToCanvasPosition(m_Canvas, gameObject.transform.position, m_WCamera, false);
        gameObject.SetParent(m_Parent);
        gameObject.anchoredPosition = ViewportPosition;
    }
    public Vector3 CanvasPositonOf(RectTransform rectTransform)
    {
        Vector3 worldPos = new();
        m_UICamera = CameraMain.instance.GetCam();
        Debug.Log($"Rect Position :{rectTransform.position}");
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Canvas.GetComponent<RectTransform>(), rectTransform.position ,m_UICamera,out Vector2 recPos);
        worldPos = m_Canvas.GetComponent<RectTransform>().TransformPoint(recPos);
        Debug.Log($"worldPos {worldPos}");
        return worldPos;
    }
}
