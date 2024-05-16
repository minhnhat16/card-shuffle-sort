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

    [SerializeField] private Text lb_cost; // cost per slot ui
    [HideInInspector] public UnityEvent<int> currencyChange = new();
    [HideInInspector] public UnityEvent<bool> slotBtnClicked = new();
    private void OnEnable()
    {
        //currencyChange = IngameController.instance.onCurrencyChanged == null ? null : IngameController.instance.onCurrencyChanged;
        currencyChange?.AddListener(CheckSlotCanUnlock);
        btn.onClick?.AddListener(ClickedBuyBtn);
    }
    private void Start()
    {
        lb_cost = GetComponentInChildren<Text>();
    }
    private void CheckSlotCanUnlock(int currency)
    {
        bool isEnable = currency >= cost ? true : false;
        SetBtnEnable(isEnable);
    }
    private void SetBtnEnable(bool isOn)
    {
        btn.GetComponent<Button>().enabled = isOn;
    }
    private void OnDisable()
    {
        btn.onClick?.RemoveListener(ClickedBuyBtn);
    }
    public void InitButton(int cost, Currency currency)
    {
        if (cost <= 0)
        {
            //SetBtnType(false,currency);
            return;
        }
        else
        {
            SetPriceLable(cost);
            SetBtnType(true,currency);
        }
       
    }
    internal void SetPriceLable(int cost)
    {
        this.cost = cost;
        float percent = cost / 1000;
        if (percent <1)
        {
            lb_cost.text = cost.ToString();
        }
        else
        {
            percent = Convert.ToInt32(percent);
            lb_cost.text = percent.ToString() + "K";
        }

    }
    internal void SetBtnType(bool isActive,Currency typeCurrency)
    {
        SetImageActiveBy(isActive, (int)typeCurrency);
    }   
    
    private void SetImageActiveBy(bool isActive, int id)
    {
        for(int i = 0; i < images.Count; i++)
        {
            if (i == id) btn.gameObject.SetActive(!isActive);
            else btn.gameObject.SetActive(isActive);
        }

    }
    //TODO: SHOWING AMOUNT OF GOLD/GEM  TO UNLOCKs
    private void ClickedBuyBtn()
    {
        //TODO: USING AN AMOUNT OFF GOLD OR GEM TO UNLOCK SLOT
        Debug.Log("Clicked buy btn");
        slotBtnClicked.Invoke(true);
    }
}
