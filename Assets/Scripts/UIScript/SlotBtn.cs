using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class SlotBtn : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private int cost;
    [SerializeField] private List<Image> images; //GOLD = 0, GEM = 1, can add more currency

    [HideInInspector] public UnityEvent<int> currencyChange = new();
    private void OnEnable()
    {
        //currencyChange = IngameController.instance.onCurrencyChanged == null ? null : IngameController.instance.onCurrencyChanged;
        currencyChange?.AddListener(CheckSlotCanUnlock);
        btn.onClick?.AddListener(ClickedBuyBtn);
    }

    private void CheckSlotCanUnlock(int currency)
    {
         bool isEnable =  currency >= cost ? true: false;
        SetBtnEnable(true);
    }
    private void SetBtnEnable(bool isOn)
    {
        btn.GetComponent<Button>().enabled = isOn;
    }
    private void OnDisable()
    {
        btn.onClick?.RemoveListener(ClickedBuyBtn);
    }
   
    internal void SetBtnType(Currency typeCurrency)
    {
        SetImageActiveBy(true,(int)typeCurrency);
    }   
    
    private void SetImageActiveBy(bool isActive, int id)
    {
        for(int i = 0; i < images.Count; i++)
        {
            if (i == id) btn.gameObject.SetActive(!isActive);
            else btn.gameObject.SetActive(isActive);
        }
       
    }
    private void ClickedBuyBtn()
    {
        Debug.Log("Clicked buy btn");
    }
}
