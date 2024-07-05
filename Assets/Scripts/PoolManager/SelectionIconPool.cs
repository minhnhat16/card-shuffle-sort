using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionIconPool : MonoBehaviour
{
    public static SelectionIconPool Instance;
    public BY_Local_Pool<SelectionIcon> pool;
    public SelectionIcon prefab;
    public int total;
    public RectTransform anchor;
    public Vector3 percent;
    private void Awake()
    {
        Instance = this;
        pool = new BY_Local_Pool<SelectionIcon>(prefab, total, this.transform);
    }
    private void Start()
    {
        percent = anchor.position;
        GameObject a;   
        for (int i = 0; i < total; i++)
        {
            a= pool.list[i].gameObject;
            a.transform.SetPositionAndRotation(percent,Quaternion.identity);
            a.SetActive(true);
            percent.x += 0.1f;
        }
    }
}
