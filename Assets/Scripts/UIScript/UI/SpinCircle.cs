using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class SpinCircle : MonoBehaviour
{
    [SerializeField] RectTransform rect;
    [SerializeField] float radius;
    [SerializeField] int numberOfObjects;
    [SerializeField] int minusStep;
    [SerializeField] List<SpinItem> _items;
    [SerializeField] List<float> angleSteps;
    [SerializeField] float angleCheck;
    [SerializeField] SpinConfig spinConfig;
    [SerializeField] SpinItem crItem;
    [SerializeField] Button button;
    [SerializeField] Text claim_lb;
    [SerializeField] bool isSpining;
    [SerializeField] RadialLayout radialLayout;

    public UnityEvent<bool> spinnedEvent = new UnityEvent<bool>();


    public bool IsSpining { get { return isSpining; } set { IsSpining = value; } }
    // Start is called before the first frame update
    private void OnEnable()
    {
        spinConfig = ConfigFileManager.Instance.SpinConfig;
    }
    private void OnDisable()
    {
        
    }
    void Start()
    {
        isSpining = true;
        SpawnObjectsInCircle();
        radialLayout = GetComponentInChildren<RadialLayout>();

    }

    public void SpinningCircle()
    {
        isSpining = true;
        button.gameObject.SetActive(false);
        angleSteps = radialLayout.radials;
        // FUNCT CALCULATE WHERE THE ITEM ON THEN ROTATE TO THAT ITEM POST
        float vect = AngleCalculator();
        Debug.Log("VECT " + vect);
        Tween circleSpin = transform.DORotate(new Vector3(0, 0, vect + 360 * 10), 5, RotateMode.FastBeyond360);
        circleSpin.OnPlay(() =>
        {
            //SoundManager.instance.PlaySFX(SoundManager.SFX.SpinSFX);
        });
        circleSpin.OnComplete(() => 
        {
            isSpining = false;
            crItem.OnRewardItem();
            claim_lb.text = $"Congratuation you get {crItem.Amount} of {crItem.Type}";
            spinnedEvent?.Invoke(true);
        });
    }
    // FUNTION FIND ITEM'S ANGLE
    public float AngleCalculator()
    {
        int random = Random.Range(0, 8);
        crItem  = _items[random];
        float newAngle = angleCheck = angleSteps[random] -90;
        return newAngle;
    }
 
    public void SpawnObjectsInCircle()
    {
        Debug.Log("SpawnObjectsInCircle");
        float angleStep = 360f / numberOfObjects;
        var allSpinConfig = spinConfig.GetAllRecord();
        for (int i = 0; i < allSpinConfig.Count; i++)
        {
            InitiateNewSpinItem(i);
        }
        
    }
    private void InitiateNewSpinItem(int i)
    {
        GameObject item = Instantiate(Resources.Load("Prefabs/UIPrefab/SpinItem", typeof(GameObject)), rect) as GameObject;
        var itemConfig = spinConfig.GetRecordByKeySearch(i);
        item.GetComponent<SpinItem>().InitItem(itemConfig);
        _items.Add(item.GetComponent<SpinItem>());
    }
}
