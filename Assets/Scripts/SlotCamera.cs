using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SlotCamera : MonoBehaviour
{
    public static SlotCamera instance;
    private Camera s_Camera;
    public float height;
    public float width;
    public GameObject _obj;
    private const float baseAspect =1;
    public float rate;
    [SerializeField] private float mul_Time = 0.5f;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        s_Camera =GetComponent<Camera>();
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
    private void MultipleSizeByTime(float targetSize)
    {
        if (targetSize <= s_Camera.orthographicSize) return ;
        float diff =0 ;
        do
        {
            diff += s_Camera.orthographicSize + Time.deltaTime * mul_Time;
        } while(diff< targetSize);
    }
}
