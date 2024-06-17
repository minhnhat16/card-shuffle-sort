using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialsScript : MonoBehaviour
{
    [SerializeField] List<Collider2D> stepList;
    [SerializeField] int currentStep;

    // Start is called before the first frame update
    void Start()
    {
        currentStep = 0;
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
        Player.Instance.PlayerTouchTutorial(stepList[currentStep]);
        //Debug.Log("Tutorial completed!");
    }
}
