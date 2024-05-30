

public enum ViewIndex
{
    EmptyView = 0,
    LoadingView = 1,
    MainScreenView = 2,
    GamePlayView = 3,
    ShopView = 5,
    CollectionView = 6,
    DailyView = 7,
    WardrobeView = 8,   
    RankView = 9
}

public class ViewParam { }

public class LoadingViewParam : ViewParam
{ 
}
public class GamePlayViewParam : ViewParam
{
    public int currentCardCount;
    public int maxCardCount;
    public string currentTime;
    public string lastTime;
}
public class MainScreenViewParam : ViewParam
{
    public int totalGold;
}
public class ViewConfig
{
    public static ViewIndex[] viewArray = {
        ViewIndex.EmptyView,
        ViewIndex.LoadingView,
        ViewIndex.MainScreenView,
        ViewIndex.GamePlayView,
        ViewIndex.ShopView,
        ViewIndex.DailyView,
        ViewIndex.CollectionView,
        ViewIndex.WardrobeView,
        ViewIndex.RankView
    };
}