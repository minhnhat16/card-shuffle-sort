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
    private void OnEnable(){
        //onCurrentStepClicked.AddListener(CurrenStepClicked);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentStep = 0;
        onCurrentStepClicked = stepList[currentStep].onStepClicked;
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
        Player.Instance.PlayerTouchTutorial(stepList[currentStep], ()=> 
        {
            stepList[currentStep].gameObject.SetActive(false);
            int nextStep = ++currentStep;
            if(nextStep > stepList.Count)
            {
                Debug.Log("GO TO FINAL STEPP");
            }
            if (stepList[nextStep].Type != TutorialEnum.Final)
            {
                stepList[nextStep].gameObject.SetActive(true);
                Debug.Log($"Next step active true {nextStep} tpye {stepList[nextStep].Type}");
            }
            else
            {
                GameManager.instance.IsNewPlayer = false;
                gameObject.SetActive(false);
            }
        });
        //Debug.Log("Tutorial completed!");
    }

}
