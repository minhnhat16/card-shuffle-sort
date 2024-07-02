using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

public class LoadingView : BaseView
{
    public Slider loadingProgress;
    public Text loaddingText;
    private float t1 = 0;
    [SerializeField] SkeletonGraphic logo;
    public override void Setup(ViewParam viewParam)
    {
        base.Setup(viewParam);
    }
    public override void OnStartShowView()
    {
        base.OnStartShowView();
        //logo = GetComponentInChildren<SkeletonGraphic>();
        //PlayStartingAnimation()
    }
    private void Update()
    {
        UpdateLoadingProgress();
    }
    private void PlayStartingAnimation()
    {
        // Get the AnimationState from the SkeletonAnimation component
        var animationState = logo.AnimationState;

        // Check if the starting animation exists
        if (animationState.Data.SkeletonData.FindAnimation("out") != null)
        {
            // Set the animation to the starting animation and play it
            animationState.SetAnimation(0, "out", true);
        }
        else
        {
            // Log a warning if the starting animation doesn't exist
            //Debug.LogWarning("Starting animation not found!");
        }
    }
    private void UpdateLoadingProgress()
    {
        loadingProgress.value = LoadSceneManager.instance.progress + 0.2f;

        t1 += Time.deltaTime;
        if (t1 >= 0.2f && t1 < 0.4f)
        {
            loaddingText.text = "Loading..";
        }
        else if (t1 >= 0.4f && t1 < 0.6f)
        {
            loaddingText.text = "Loading...";
        }
        else if (t1 >= 0.6f)
        {
            loaddingText.text = "Loading.";
            t1 = 0;
        }
    }
}
