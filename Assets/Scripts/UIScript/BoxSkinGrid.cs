using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoxSkinGrid : MonoBehaviour
{
    public static BoxSkinGrid Instance;
    
    private void Awake()
    {
        if (Instance != null)
            Instance = this;
    }
    private void Start()
    {

    }
    private void OnEnable()
    {
       
    }
    private void OnDisable()
    {
    }
    public void StartSetupItem()
    {
    }
    public void SetupBoxGrid()
    {
    }
    private void InitiateSkinItem()
    {
    }

}