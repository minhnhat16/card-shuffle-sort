using UnityEngine;

public class TabPlay : MonoBehaviour
{
    public GameObject tabOn;
    public GameObject tabOff;
    public Animator animator;

    public void OnClickTabOn()
    {
        tabOn.SetActive(true);
        tabOff.SetActive(false);
        animator.Play("TabOn");
        SoundManager.instance.PlaySFX(SoundManager.SFX.UIClickSFX);
        //DialogManager.Instance.ShowDialog(DialogIndex.LabelChooseDialog);
        ViewManager.Instance.SwitchView(ViewIndex.MainScreenView);
    }
    public void OnTabOff()
    {
        tabOn.SetActive(false);
        tabOff.SetActive(true);
    }
}
