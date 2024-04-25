using System;

public enum DialogIndex
{
    LabelChooseDialog = 0,
    LoseDialog = 1,
    DailyRewardDialog = 2,
    ReviveDialog = 3,
    BuyConfirmDialog = 4,
    ItemConfirmDialog = 5,
    SettingDialog = 6,
    RateDialog = 7,
}

public class DialogParam { }

public class SettingDialogParam : DialogParam
{
    public bool musicSetting;
    public bool sfxSetting;
}
public class ReviveDialogParam : DialogParam
{
    public int levelNum;
}

public class LoseDialogParam : DialogParam
{
    public int score;
}

public class DailyDialogParam : DialogParam
{
    int totalDay;
    bool isClaimed;
}

public class BuyConfirmDialogParam : DialogParam
{
    public Action onConfirmAction;
    public Action onCancleAction;
    public string amount_lb;
    public string bonus_lb;
    public string cost_lb;
    public int cost;
    public string plaintext;
}

public class ItemConfirmParam : DialogParam
{
    public bool isAds;
    public ItemType type;
    public string name;
}


public class DailyParam : DialogParam
{
    int currenReward;
}
public class RateParam : DialogParam
{

}
public class DialogConfig
{
    public static DialogIndex[] dialogArray = {
       DialogIndex.LabelChooseDialog,
       DialogIndex.LoseDialog,
       DialogIndex.DailyRewardDialog,
       DialogIndex.ReviveDialog,
       DialogIndex.BuyConfirmDialog,
       DialogIndex.ItemConfirmDialog,
       DialogIndex.SettingDialog,
       DialogIndex.RateDialog,
    };
}
