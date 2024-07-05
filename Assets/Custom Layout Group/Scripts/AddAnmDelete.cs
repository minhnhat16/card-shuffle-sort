using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test class, nothing useful here
/// </summary>
public class AddAnmDelete : MonoBehaviour
{
    public GameObject template;
    public Transform parent;

    public void Add() {
        for(int i = 0; i < 3; i++) {
            Instantiate(template, parent);
        }
    }

    public void Remove() {
        for (int i = 0; i < Mathf.Min(2, parent.childCount); i++) {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
