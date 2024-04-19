using UnityEngine;

public class LastDailyItem : DailyItem
{

    // Start is called before the first frame update
    private void OnEnable()
    {
        var parent = FindObjectOfType<DailyRewardDialog>();
        if (parent != null)
        {
            onClickDailyItem = parent.onClickDailyItem;
            onItemClaim = parent.onClickClaim;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DebugButton()
    {
        Debug.Log("On Click Daily Item");
    }
    public override void SwitchType(IEDailyType type)
    {
        currentType = type;
        daily_btn.enabled = true;
        switch (type)
        {
            case IEDailyType.Available:
                backgrounds[0].SetActive(true);
                backgrounds[1].SetActive(false);
                backgrounds[2].SetActive(false);
                daily_btn.enabled = true;
                break;
            case IEDailyType.Unavailable:
                backgrounds[1].SetActive(true);
                //daily_btn.gameObject.SetActive(false);
                break;
            case IEDailyType.Claimed:
                backgrounds[1].SetActive(false);
                backgrounds[0].SetActive(false);
                backgrounds[2].SetActive(true);
                daily_btn.enabled = false;
                Amount_lb.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }
    public override void OnClickDailyItem()
    {
        Debug.Log("On Click Daily Item");
        if (currentType == IEDailyType.Available)
        {
            //var parent = DialogManager.Instance.dicDialog[DialogIndex.DailyRewardDialog].GetComponent<DailyRewardDialog>();
            //   parent.dailyGrid.currentDaily = this;
            onClickDailyItem?.Invoke(true);
        }
        else
        {
            onClickDailyItem?.Invoke(false);
        }
    }
}
