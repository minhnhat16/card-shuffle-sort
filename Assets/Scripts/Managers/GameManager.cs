using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private IngameController ingameController;
    [SerializeField] private DayTimeController dayTimeController;
    public List<CardColorPallet> listCurrentCardColor;
    public UIRootControlScale UIRoot;
    [SerializeField] private int languageID;
    [SerializeField] private int trackLevelStart;
    [SerializeField] private int cardPool;

    public int TrackLevelStart { get=> trackLevelStart; set => trackLevelStart = value; }
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        UIRoot = GetComponentInParent<UIRootControlScale>();
        DOTween.SetTweensCapacity(1000, 125);
        //ingameController.gameObject.SetActive(false);
    }

    public void GetCardListColorFormData( CardType currentType)
    {
        listCurrentCardColor = new();
        var colorData = DataAPIController.instance.GetDataColorByType(currentType);
        listCurrentCardColor = colorData.color;
    }

    // Update is called once per frame
    public void SetupGameManager()
    {
        //ingameController = FindObjectOfType<IngameController>();
        dayTimeController = FindObjectOfType<DayTimeController>();
        dayTimeController.enabled = true;
        //dayTimeController.CheckNewDay();
    }
    public void LoadIngameSence()
    {
        //ingameController.enabled = true;
        ingameController.gameObject.SetActive(true);
        ingameController.Init();
        //CameraMain.instance.main.gameObject.SetActive(true);
    }
    public void SetUpIngame()
    {
        dayTimeController.StartCoroutine(dayTimeController.InitCouroutine());
    }
    public string DevideCurrency(int currency)
    {
        if (currency < 10000) return currency.ToString();
        else
        {
            currency /= 1000;
            currency.ToString();
            return $"{currency}k";
        }
    }
}
