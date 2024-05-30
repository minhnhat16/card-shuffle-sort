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
        rect = GetComponent<RectTransform>();
        SpawnObjectsInCircle();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpinningCircle()
    {
        isSpining = true;
        button.gameObject.SetActive(false);
        // FUNCT CALCULATE WHERE THE ITEM ON THEN ROTATE TO THAT ITEM POST
        float vect = AngleCalculator();
        //vect = Mathf.Clamp(vect, 180, -180);
        Debug.Log("VECT " + vect);
        Tween circleSpin = transform.DORotate(new Vector3(0, 0, vect + 360 * 10), 5, RotateMode.FastBeyond360);
        circleSpin.OnPlay(() =>
        {
            SoundManager.instance.PlaySFX(SoundManager.SFX.SpinSFX);
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
        int random = Random.Range(0, 7);
        crItem  = _items[random];
        float newAngle = angleCheck = 85 - angleSteps[random];
        return newAngle;
    }
 
    public void SpawnObjectsInCircle()
    {
        Debug.Log("SpawnObjectsInCircle");
        float angleStep = 360f / numberOfObjects;
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 localPost = LocalItemPosition(i,angleStep,radius);
            Vector3 spawnPosition =  rect.TransformPoint(localPost);
            Debug.Log($"Spawn Position item spin {spawnPosition}");
            Quaternion spawnRotation = Quaternion.Euler(0f, 0f, angleSteps[i] +80);

            InitiateNewSpinItem(i,spawnRotation,spawnPosition);
        }
    }
    private void InitiateNewSpinItem(int i,Quaternion rotation,Vector3 pos)
    {
        GameObject item = Instantiate(Resources.Load("Prefab/UIPrefab/SpinItem", typeof(GameObject)), pos, rotation, this.transform) as GameObject;
        var itemConfig = spinConfig.GetRecordByKeySearch(i);
        item.GetComponent<SpinItem>().InitItem(itemConfig);
        _items.Add(item.GetComponent<SpinItem>());
    }
    private Vector3 LocalItemPosition(int i,float angleStep, float radius)
    {
        float angle = i * angleStep - minusStep;
        float x = Mathf.Cos(Mathf.Deg2Rad * (angle)) * radius;
        float y = Mathf.Sin(Mathf.Deg2Rad * (angle)) * radius;
        angleSteps.Add(angle);
        return new Vector3(x, y, 0f);
    }
}
