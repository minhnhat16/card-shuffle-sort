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
        //ingameController.enabled = true
        //SetUpCamera();
    }
    public void SetUpCamera()
    {
        GameObject camObject = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObject == null)
        {
            //Debug.LogError("camObject null");
            CameraMain.instance.GetCamera();
            CameraMain.instance.GetCameraAspect();
        }
        else
        {
            CameraMain.instance.main = camObject.GetComponent<Camera>();
            CameraMain.instance.GetCameraAspect();
        }
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
