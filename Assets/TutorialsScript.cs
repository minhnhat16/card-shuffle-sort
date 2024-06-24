using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TutorialsScript : MonoBehaviour
{
    [SerializeField] List<TutorialStep> stepList;
    [SerializeField] int currentStep;

    [HideInInspector] public UnityEvent<bool> onCurrentStepClicked = new();
    public GameObject cusor;
    private void OnEnable() {
        //onCurrentStepClicked.AddListener(CurrenStepClicked);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentStep = 0;
        onCurrentStepClicked = stepList[currentStep].onStepClicked;
        CusorStepping(stepList[currentStep]);
        if (GameManager.instance.IsNewPlayer)
        {
            stepList[currentStep].gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        TutorialCourountine();
    }
    void TutorialCourountine()
    {
        StartCoroutine(Tutorial());
    }

    private IEnumerator Tutorial()
    {
        // Wait until the Player instance is initialized
        yield return new WaitUntil(() => Player.Instance != null);
        Player.Instance.PlayerTouchTutorial(stepList[currentStep], () =>
        {
            stepList[currentStep].gameObject.SetActive(false);
            int nextStep = ++currentStep;
          
            if (nextStep > stepList.Count)
            {
                Debug.Log("GO TO FINAL STEPP");
            }
            if (stepList[nextStep].Type != TutorialEnum.Final)
            {
                stepList[nextStep].gameObject.SetActive(true);
                if (stepList[nextStep].Type == TutorialEnum.StepThree /*||
                    stepList[nextStep].Type == TutorialEnum.StepFive*/)
                {
                    Debug.Log("If next stepp 4");
                    CusorStepping(stepList[nextStep]);
                }
                else
                {
                    Debug.Log("If next stepp not 4");
                    CusorStepping(stepList[++nextStep]);
                }
                Debug.Log($"Next step active true {nextStep} tpye {stepList[nextStep].Type}");
            }
            else
            {
                GameManager.instance.IsNewPlayer = false;
                DataAPIController.instance.SetPlayerNewAtFalse(() =>
                {
                    CusorStepping(stepList[nextStep]);
                    gameObject.SetActive(false);
                });
                
            }
        });
        //Debug.Log("Tutorial completed!");
    }
    public void CusorStepping(TutorialStep step)
    {
        Debug.Log("STEPP" + step.Type);
        Vector3 cusorPos = step.transform.position + new Vector3(0.5f, -1, 0);
        cusor.transform.DOMove(cusorPos, 0.1f);
    }
}
