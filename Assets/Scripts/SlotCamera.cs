using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SlotCamera : MonoBehaviour
{
    private Camera s_Camera;
    [SerializeField] private float mul_Time = 0.5f;
    private void Start()
    {
        s_Camera =GetComponent<Camera>(); 
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
