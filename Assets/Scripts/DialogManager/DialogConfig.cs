using System;

public enum DialogIndex
{
    LoseDialog = 0,
    DailyRewardDialog = 1,
    ReviveDialog = 2,
    BuyConfirmDialog = 3,
    ItemConfirmDialog = 4,
    SettingDialog = 5,
    RateDialog = 6,
    PickCardDialog = 7,
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
public class PickCardParam : DialogParam
{

}
public class DialogConfig
{
    public static DialogIndex[] dialogArray = {
       DialogIndex.LoseDialog,
       DialogIndex.DailyRewardDialog,
       DialogIndex.ReviveDialog,
       DialogIndex.BuyConfirmDialog,
       DialogIndex.ItemConfirmDialog,
       DialogIndex.SettingDialog,
       DialogIndex.RateDialog,
       DialogIndex.PickCardDialog,
    };
}
