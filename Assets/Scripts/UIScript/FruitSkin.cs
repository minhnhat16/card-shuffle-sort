using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FruitSkin : MonoBehaviour
{
    [SerializeField] private Image fruitSkin_img;
    [SerializeField] private Material grayScale;
    // Start is called before the first frame update
    void Start()
    {
        fruitSkin_img = GetComponent<Image>();
        MaterialTaking();
        SetGrayScale(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void MaterialTaking()
    {
        if(fruitSkin_img != null && fruitSkin_img.material != null)
        {
            grayScale = fruitSkin_img.material; 
        }
    }
    void SetGrayScale(float value)
    {
        fruitSkin_img.material.SetFloat("_GrayScaleAmount", value);
    } 
}
