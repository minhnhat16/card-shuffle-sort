using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    [SerializeField] Button btn_secondBG;
    [SerializeField] Text claim_lb;
    [SerializeField] bool isSpining;
    [SerializeField] RadialLayout radialLayout;
    [SerializeField] RectTransform secondBG;
    [SerializeField] RectTransform rewardAnchor;
    [SerializeField] Vector3 anchorItemReward;
    [SerializeField] ParticleSystem rewardParticle;
    public UnityEvent<bool> spinnedEvent = new UnityEvent<bool>();
    bool hasPlayedSound;
    public bool IsSpining { get { return isSpining; } set { IsSpining = value; } }

    public RectTransform SecondBG { get => secondBG; set => secondBG = value; }

    private void OnEnable()
    {
        spinConfig = ConfigFileManager.Instance.SpinConfig;
        btn_secondBG.onClick.AddListener(OnRewarded);
    }

    private void OnDisable()
    {
        btn_secondBG.onClick.RemoveListener(OnRewarded);
    }

    void Start()
    {
        isSpining = true;
        radialLayout = GetComponentInChildren<RadialLayout>();
        btn_secondBG.interactable = false;
        StartCoroutine(SpawnObjectsInCircle());
    }

    IEnumerator SpawnObjectsInCircle()
    {
        Debug.Log("SpawnObjectsInCircle");
        var allSpinConfig = spinConfig.GetAllRecord();
        GameObject prefab = Resources.Load("Prefabs/UIPrefab/SpinItem") as GameObject;
        for (int i = 0; i < allSpinConfig.Count; i++)
        {
            yield return StartCoroutine(InitiateNewSpinItem(i,prefab));
        }
    }

    IEnumerator InitiateNewSpinItem(int i,GameObject prefab)
    {
        GameObject item = Instantiate(prefab, rect);
        var itemConfig = spinConfig.GetRecordByKeySearch(i);
        item.GetComponent<SpinItem>().InitItem(itemConfig);
        angleSteps.Add(item.GetComponent<RectTransform>().eulerAngles.z);
        _items.Add(item.GetComponent<SpinItem>());
        yield return null; // Ensures one frame passes before continuing
    }

    public void SpinningCircle()
    {
        isSpining = true;
        button.interactable = false;
        button.gameObject.SetActive(true);
        angleSteps = radialLayout.radials;
        float vect = AngleCalculator();
        Debug.Log("VECT " + vect);
        Tween circleSpin = transform.DORotate(new Vector3(0, 0, (360 - vect) + 360 * 10), 5, RotateMode.FastBeyond360);
        circleSpin.OnPlay(() =>
        {
            float z = GetComponent<RectTransform>().rotation.z;
            DataAPIController.instance.SetSpinTimeData(DateTime.Now);
        });

        circleSpin.OnUpdate(() =>
        {
            float currentAngle = GetComponent<RectTransform>().eulerAngles.z;
            if (Mathf.Abs(currentAngle % 45) < 20f)
            {
                if (!hasPlayedSound)
                {
                    SoundManager.instance.PlaySFX(SoundManager.SFX.SpinSFX);
                    hasPlayedSound = true;
                }
            }
            else
            {
                hasPlayedSound = false;
            }
        });

        circleSpin.OnComplete(() =>
        {
            isSpining = false;
            SecondBG.gameObject.SetActive(true);
            var cloneItem = Instantiate(crItem, Vector3.zero, Quaternion.identity, transform.parent);
            cloneItem.GetComponent<RectTransform>().anchoredPosition3D = rewardAnchor.anchoredPosition3D;
            cloneItem.GetComponent<RectTransform>().anchorMin = cloneItem.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            cloneItem.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            cloneItem.GetComponent<RectTransform>().DOAnchorPos(anchorItemReward, 1.5f);
            cloneItem.ItemRewarding();
            crItem.OnRewardItem();
            Debug.Log($"Congratuation you get {crItem.Amount} of {crItem.Type}");
            claim_lb.text = $"Congratuation you get {crItem.Amount} of {crItem.Type}";
            SwitchParticleCase(crItem.Type);
            btn_secondBG.interactable = true;
            spinnedEvent?.Invoke(true);
        });
    }

    public float AngleCalculator()
    {
        int random = Random.Range(0, 8);
        crItem = _items[random];
        float newAngle = angleCheck = angleSteps[random];
        return newAngle;
    }

    private void SwitchParticleCase(SpinEnum type)
    {
        var textureSheetAnimation = rewardParticle.textureSheetAnimation;
        Debug.Log($"Type{(int)type}");
        if (type != SpinEnum.Bonus)
        {
            int intType = (int)type;
            textureSheetAnimation.startFrame = new ParticleSystem.MinMaxCurve(intType);
        }
        else
        {
            textureSheetAnimation.startFrame = new ParticleSystem.MinMaxCurve(0, 4);
        }

        rewardParticle.Play();
    }

    void OnRewarded()
    {
        DialogManager.Instance.HideDialog(DialogIndex.SpinDialog);
        DataAPIController.instance.SetSpinData(isSpinned: true);
    }
}
