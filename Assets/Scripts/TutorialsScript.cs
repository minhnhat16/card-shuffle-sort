using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialsScript : MonoBehaviour
{
    public GameObject cusor;
    [SerializeField] int currentStep;

    [HideInInspector] public UnityEvent<bool> onCurrentStepClicked = new();
    [SerializeField] List<TutorialStep> stepList;
    private UnityEvent<bool> reachedGoldTarget = new();
    private Slot slot;

    private void OnEnable()
    {
        reachedGoldTarget.AddListener(ActiveUnlockStep);
        //onCurrentStepClicked.AddListener(CurrenStepClicked);
        DataTrigger.RegisterValueChange(DataPath.GOLDINVENT, data =>
        {
            if (data == null) return;
            CurrencyWallet newData = data as CurrencyWallet;
            int gold = newData.amount;
            if (slot != null && gold >= slot.UnlockCost)
            {
                if (slot.status == SlotStatus.Active)
                {
                    return;
                    //DataTrigger.UnRegisterValueChange(DataPath.GOLDINVENT, data => data = 0);
                }
                reachedGoldTarget.Invoke(true);

            }
        });
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        currentStep = 0;
        onCurrentStepClicked = stepList[currentStep].onStepClicked;
        yield return new WaitUntil(() => IngameController.instance != null && !IngameController.instance.IsSortedSlotIsEmty());

        slot = IngameController.instance.TakeSlotByIndex(3);
        CusorStepping(stepList[currentStep]);
        if (GameManager.instance.IsNewPlayer)
        {
            stepList[currentStep].gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        TutorialCourountine();
    }
    // Update is called once per frame

    void TutorialCourountine()
    {
        if (!GameManager.instance.IsNewPlayer)
        {
            return;
        }
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
                //Debug.Log("GO TO FINAL STEPP");
            }
            if (stepList[nextStep].Type != TutorialEnum.Final && stepList[nextStep].Type != TutorialEnum.StepUnlock)
            {
                stepList[nextStep].gameObject.SetActive(true);
                if (stepList[nextStep].Type == TutorialEnum.StepThree)
                {
                    //Debug.Log("If next stepp 4");
                    CusorStepping(stepList[nextStep]);
                }
                else
                {
                    //Debug.Log("If next stepp not 4");
                    CusorStepping(stepList[++nextStep]);
                }
            }
            else if (stepList[currentStep].Type == TutorialEnum.StepUnlock)
            {
                cusor.SetActive(false);
                GameManager.instance.IsNewPlayer = false;
                stepList[currentStep].gameObject.SetActive(false);
                DataAPIController.instance.SetPlayerNewAtFalse(() =>
                {
                    CusorStepping(stepList[nextStep]);
                    //gameObject.SetActive(false);
                });
            }
            else if (stepList[currentStep].Type == TutorialEnum.Final || stepList[currentStep].Type == TutorialEnum.StepUnlock)
            {
                GameManager.instance.IsNewPlayer = false;
                Debug.LogWarning("If next stepp " + (stepList[nextStep].Type == TutorialEnum.Final));
                stepList[currentStep].gameObject.SetActive(false);
                
            }
        });
        //Debug.Log("Tutorial completed!");
    }
    public void CusorStepping(TutorialStep step)
    {
        //Debug.Log("STEPP" + step.Type);
        Vector3 cusorPos = step.transform.position + new Vector3(0.5f, -1, 0);
        cusor.transform.DOMove(cusorPos, 0.1f);
    }
    public void ActiveUnlockStep(bool isGoldReachTarget)
    {
        if (isGoldReachTarget)
        {
            GameManager.instance.IsNewPlayer = true;
            currentStep = (int)TutorialEnum.StepFive ;
            var unlockS = stepList[currentStep]; //unlockS == unlock step
            unlockS.gameObject.SetActive(true);
            CusorStepping(unlockS);
        }
    }
}
