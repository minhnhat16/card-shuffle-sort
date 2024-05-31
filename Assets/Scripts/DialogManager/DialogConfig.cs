using System;

public enum DialogIndex
{
    PickCardDialog =0,
    DailyRewardDialog = 1,
    BuyConfirmDialog = 2,
    ItemConfirmDialog =3,
    SettingDialog = 4,
    LableChooseDialog = 5,
    RateDialog = 6,
    LoseDialog = 7,
    SpinDialog = 8,
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
public class SpinParam : DialogParam
{
    int totalDay;
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
    public CardColorPallet premium;
    public CardColorPallet free;
}
public class DialogConfig
{
    public static DialogIndex[] dialogArray = {
       DialogIndex.PickCardDialog,
       DialogIndex.DailyRewardDialog,
       DialogIndex.BuyConfirmDialog,
       DialogIndex.ItemConfirmDialog,
       DialogIndex.SettingDialog,
       DialogIndex.LableChooseDialog,
       DialogIndex.RateDialog,
       DialogIndex.LoseDialog,
       DialogIndex.SpinDialog,
    };
}
