using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class TutorialStep : MonoBehaviour
{
    [SerializeField] private TutorialEnum type;
    public new Collider2D collider;
    [HideInInspector] public UnityEvent<bool> onStepClicked = new();
    public GameObject mask;
    public TutorialEnum Type {get{return type;} set{type = value;}}
    
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
    
    public void PlayerHit(Action callback)
    {
        Debug.Log("Player hit on stepp");
       var activeSLot=  IngameController.instance.GetListSlotActive();

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

