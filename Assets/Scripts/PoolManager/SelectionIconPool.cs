using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionIconPool : MonoBehaviour
{
    public static SelectionIconPool Instance;
    public BY_Local_Pool<SelectionIcon> pool;
    public SelectionIcon prefab;
    public int total;
    private void Awake()
    {
        Instance = this;
        pool = new BY_Local_Pool<SelectionIcon>(prefab, total, this.transform);
    }
    private void Start()
    {
        for (int i = 0; i < total; i++)
        {
            pool.SpawnNonGravityWithIndex(i);
        }
    }
}
