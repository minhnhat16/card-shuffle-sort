using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class TutorialStep : MonoBehaviour
{
    [SerializeField] private TutorialEnum type;
    public new Collider2D collider;
    [HideInInspector] public UnityEvent<bool> onStepClicked = new();
    public GameObject mask;
    [SerializeField] private Text tutotext;
    public TutorialEnum Type {get{return type;} set{type = value;}}

    public Text Tutotext { get => tutotext; set => tutotext = value; }


    // Start is called before the first frame update
    public virtual void Start()
    {
        // Initialization code if needed
        mask = GetComponentInChildren<SpriteMask>().gameObject;
        Vector3 currentScale = mask.transform.lossyScale;
        mask.transform.localScale = Vector3.zero;
        mask.transform.DOScale(currentScale, 0.25f);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        // Update logic if needed
    }

    // Method to add a collider to the list
    public void AddCollider(Collider2D collider)
    {
        if (this.collider != null)
        {
            this.collider = collider;
        }
    }
    public IEnumerator Init(int index)
    {
        --index;
        yield return new WaitUntil(() => ViewManager.Instance != null);
        yield return new WaitUntil(() => ViewManager.Instance.currentView.viewIndex == ViewIndex.GamePlayView);
        Debug.Log("Init tutorial step " +index);
        ViewManager.Instance.dicView.TryGetValue(ViewIndex.GamePlayView, out BaseView gamePlayViewObj);
        InitTutotext(index, gamePlayViewObj);
    }
    void InitTutotext(int index, BaseView view)
    {
        view.TryGetComponent<GamePlayView>(out GamePlayView gamePlay);
        tutotext.rectTransform.SetParent(gamePlay.AnchorTutorials[index]);
        tutotext.rectTransform.anchoredPosition3D = Vector3.zero;
        tutotext.rectTransform.localScale = Vector3.one;
        tutotext.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f),1f,1,100).SetLoops(100);
    }
    public void PlayerHit(Action callback)
    {
        Debug.Log("Player hit on stepp");
        var activeSLot=  IngameController.instance.GetListSlotActive();
        tutotext.rectTransform.SetParent(transform);
        tutotext.gameObject.SetActive(false);
        switch (type)
        {
            case TutorialEnum.StepOne:
                activeSLot[0].TapHandler();
                callback?.Invoke();
                break;
            case TutorialEnum.StepTwo:
                activeSLot[1].TapHandler();
                callback?.Invoke();
                break;
            case TutorialEnum.StepThree:
                activeSLot[1].TapHandler();
                callback?.Invoke();
                break;
            case TutorialEnum.SteppFourth:
                IngameController.instance.dealerParent.Dealers[0].dealSlot.TapHandler();
                callback?.Invoke();
                break;
            case TutorialEnum.StepFive:
                callback?.Invoke();
                break;
            case TutorialEnum.Final:
                callback?.Invoke();
                break;
            default:
                break;
        }
    }
}

