using UnityEngine;
using UnityEngine.UI;

public class LoadingView : BaseView
{
    public Slider loadingProgress;
    public Text loaddingText;
    private float t1 = 0;
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
  
    private void UpdateLoadingProgress()
    {
        loadingProgress.value = LoadSceneManager.instance.progress + 0.01f;
        t1 += Time.deltaTime * 0.25f;
        
         if (t1 >= 0.0f && t1 < 0.2f)
        {
            loaddingText.text = "Loading..";
        }
        else if (t1 >= 0.2f && t1 < 0.4f)
        {
            loaddingText.text = "Loading...";
        }
        else if (t1 >= 0.4f && t1 < 0.6f)
        {
            loaddingText.text = "Loading.";
            t1 = 0;
        }
        else if(t1 >=0.6f)
        {
            loaddingText.text = "Loading";
        }
    }
}
