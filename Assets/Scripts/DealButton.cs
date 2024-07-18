using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DealButton : MonoBehaviour
{
    public RectTransform spawnPoint;
    public const int spawnSize = 5;
    [SerializeField] private bool isCountingTime;
    [SerializeField] private int seconds;
    [SerializeField] private int minutes;
    [SerializeField] private int targetMinutes;
    [SerializeField] private int targetSeconds;
    [SerializeField] private int currentCardCounter;
    [SerializeField] private int maxCardCounter;
    [SerializeField] private float floatTimeCounter ;
    [SerializeField] private float delayBtwCards = 0.075f;
    [SerializeField] private float delayBtwSlots = 0.25f;
    [SerializeField] private Text lb_cardCounter;
    [SerializeField] private Text lb_timeCounter;
    [SerializeField] private Button tapBtn;
    [SerializeField] private Vector3 spawnVect;
    [SerializeField] private string timeCounter;
    [SerializeField] private string lastTimeData;
    [SerializeField] private DateTime targetTime;
    [SerializeField] private Image fill_CardCoutn;
    [HideInInspector] public UnityEvent<bool> onCardPoolEmty = new();
    [HideInInspector] public UnityEvent<bool> onCardRechage = new();

    private void OnEnable()
    {
        tapBtn.onClick.AddListener(HandleTap);
        onCardRechage.AddListener(DoTimeCounter);
        onCardPoolEmty.AddListener(DoCardCharge);
        DataTrigger.RegisterValueChange(DataPath.CURRENTCARDPOOL, (data) =>
        {
            if (data == null) return;
            NewCouterData((int)data,30);
        });

    }

    private void OnDisable()
    {
        tapBtn.onClick.RemoveAllListeners();
    }
    private IEnumerator Start()
    {
        spawnPoint = gameObject.GetComponent<RectTransform>();
        yield return new WaitUntil(() => DataAPIController.instance.isInitDone == true);
        lastTimeData = DataAPIController.instance.GetLastTimeData();
        timeCounter = DataAPIController.instance.GetLastTimeData();
        currentCardCounter = DataAPIController.instance.CurrentCardPool();
        maxCardCounter = DataAPIController.instance.MaxCardPool();
        FillCardCounter(currentCardCounter, maxCardCounter);
        targetTime = DateTime.Parse(lastTimeData);
        onCardRechage.Invoke(targetTime > DateTime.Now);
        InvokeRepeating(nameof(UpdateCounter), 1, 1f);
    }
  
    private void UpdateCounter()
    {
        CardCounterTextUpdate(currentCardCounter, maxCardCounter);
    }
    public void DoTimeCounter(bool isTimeCounter)
    {
        isCountingTime = isTimeCounter;
        StartCoroutine(UpdateTime());
    }
    IEnumerator UpdateTime()
    {
        //Debug.Log("UPDATE TIME");
        while (true)
        {
            // Tính toán thời gian còn lại
            TimeSpan timeRemaining = targetTime - DateTime.Now;

            // Nếu thời gian đã hết, dừng bộ đếm
            if (timeRemaining.TotalSeconds <= 0)
            {
                lb_timeCounter.text = "Card is full now";
    
                // Thực hiện các hành động khác tại đây nếu cần
                isCountingTime = false;
                currentCardCounter = maxCardCounter;
                DataAPIController.instance.SetCurrrentCardPool(currentCardCounter, null);
                FillCardCounter(currentCardCounter, maxCardCounter);
                yield break;
            }

            //lb_timeCounter.text = $"500 cards in {minutes}:{seconds}";
            lb_timeCounter.text =  "500 cards in " + string.Format("{0:00}:{1:00}", timeRemaining.Minutes, timeRemaining.Seconds);
            yield return null;
        }
    }
    public void FillCardCounter(int current, int max)
    {
        float targetPercent = (float)current / (float)max;
        StartCoroutine(FillCounterOverTime(targetPercent, 1f)); // 1f là thời gian chuyển đổi
    }
    private IEnumerator FillCounterOverTime(float targetPercent, float duration)
    {
        float startPercent = fill_CardCoutn.fillAmount;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            fill_CardCoutn.fillAmount = Mathf.Lerp(startPercent, targetPercent, timeElapsed / duration);
            yield return null;
        }

        fill_CardCoutn.fillAmount = targetPercent;
    }
    public void CardCounterTextUpdate(int currentCard, int maxCard)
    {
        if (currentCard < 0) lb_cardCounter.text = $"{0}/{maxCard}";
        else
        {
            lb_cardCounter.text = $"{currentCard}/{maxCard}";
        }
    }
    public void HandleTap()
    {
        if (currentCardCounter <= 0)
        {
            onCardPoolEmty?.Invoke(true);
            tapBtn.interactable = false;
            Player.Instance.isDealBtnActive = false;
            return;
        }
        //Debug.Log("Handel tap dealbutton");
        if (Player.Instance.isAnimPlaying) return;

        if (!Player.Instance.isDealBtnActive)
        {
            //Debug.LogWarning("dealbutton active false");
            Player.Instance.isDealBtnActive = true;
        }
        if (Player.Instance.fromSlot is not null)
        {
            tapBtn.interactable = false;
            float tempY;
            foreach (var card in Player.Instance.fromSlot.GetSelectedCards())
            {
                tempY = card.transform.position.y;
                card.transform.DOMoveY(tempY - 0.1f, 0.2f);
                if (currentCardCounter <= 0)
                {
                    onCardPoolEmty?.Invoke(true);
                    return;
                }
            }

            Player.Instance.fromSlot.GetSelectedCards().Clear();
            Player.Instance.fromSlot.UpdateSlotState();
            Player.Instance.isDealBtnActive = true;
            Player.Instance.isAnimPlaying = true;
            Player.Instance.fromSlot = null;
            Player.Instance.toSlot = null;
        }
        Tween t = transform.DOScaleZ(0.5f, 0.24f).OnComplete(() =>
        {
             t = transform.DOScaleZ(1.25f, 1f);
             t.OnComplete(() => t.Kill());
        });

        float timer = 0.25f;
        var listSlot = IngameController.instance.GetListSlotActive();
        int targetCardCanDeal = (int)((listSlot.Count * 5)/ currentCardCounter) ;
        foreach (var s in listSlot) 
        {
            s.SetTargetToDealCard(true);
            StartCoroutine(SendingCard(s, timer));
            timer += delayBtwSlots;
        }
        DataAPIController.instance.SetCurrrentCardPool(currentCardCounter, () =>
        {
            Player.Instance.isDealBtnActive = false;
            tapBtn.interactable = true;
        });

    }
    public void NewCouterData(int data,double target)
    {
        int CardCounterNewData = (int)data;
        if (CardCounterNewData < maxCardCounter && !isCountingTime)
        {
            targetTime = DateTime.Now.AddMinutes(target);
            DataAPIController.instance.SaveTargetTime(timeCounter, () =>
            {
                onCardRechage?.Invoke(true);
            });
        }
        FillCardCounter(currentCardCounter, maxCardCounter);
    }
    IEnumerator SendingCard(Slot s, float timer)
    {
        //Debug.Log("Card is Sending");
        yield return new WaitForSeconds(timer);
        SendCardTo(s);
    }
    private void SendCardTo(Slot destination)
    {
        float d = Player.Instance.duration;
        float offset = destination._cards.Count == 0 ? destination.transform.position.y + 0.1f : destination._cards.Last().transform.position.y + Player.Instance.cardPositionOffsetY;
        float z =  destination._cards.Count == 0 ? Player.Instance.cardPositionOffsetZ : destination._cards.Last().transform.position.z + Player.Instance.cardPositionOffsetZ;

        //destination.SetCollisionEnable(false);

        Player.Instance.isAnimPlaying = true;

        CardColorPallet targetColor = destination.TopColor();

        List<CardColorPallet> option = new(GameManager.instance.listCurrentCardColor);

        option.Remove(targetColor);

        int randomIndex = UnityEngine.Random.Range(0, option.Count);

        //Debug.Log($"random index {randomIndex}, option count {option.Count}");
        CardColorPallet spawnColor = option[randomIndex];
        CardType currentType = IngameController.instance.CurrentCardType;
        ColorConfigRecord colorRecord = ConfigFileManager.Instance.ColorConfig.GetRecordByKeySearch(spawnColor);
        float delay = 0;
        Vector3 newSpawnPoint;
        Vector3 woldPoint;
        Card c;
        Vector3 newvect = new Vector3(0, 0, 10);
        for (int i = 0; i < spawnSize; i++)
        {
            c  = CardPool.Instance.pool.SpawnNonGravity();
            c.ColorSetBy(colorRecord.Name, currentType,colorRecord.Color);
            newSpawnPoint = spawnPoint.position;
            woldPoint= ScreenToWorld.Instance.PreverseConvertPosition(newSpawnPoint);
            c.transform.SetLocalPositionAndRotation(woldPoint + spawnVect - newvect, Quaternion.identity);
            c.PlayAnimation(destination, d, Player.Instance.height, Player.Instance.ease, offset, z, delay);
            destination._cards.Add(c);
            //destination.CardColorPallets.Add(c.cardColor);
            delay += delayBtwCards;

            offset += Player.Instance.cardPositionOffsetY;
            z += Player.Instance.cardPositionOffsetZ;
            //update collision size;
            destination.SetColliderSize(1);
            currentCardCounter--;
        }
        destination.CenterCollider();
        StartCoroutine(UpdateSlotType(destination, delay + d));
    }
    private void DoCardCharge(bool onCardEmty)
    {

       bool isVideoReady =  ZenSDK.instance.IsVideoRewardReady();
        if (/*isVideoReady = */true)
        {
            OutOffCardParam param = new();
            param.targetTime = targetTime;
            DialogManager.Instance.ShowDialog(DialogIndex.OutOffCardDialog, param);
        }
    }

    IEnumerator UpdateSlotType(Slot destination, float v)
    {
        yield return new WaitForSeconds(v);
        destination.UpdateSlotState();
        Player.Instance.isAnimPlaying = false;
    }
    private void OnApplicationQuit()
    {
        DataAPIController.instance.SaveTargetTime(targetTime.ToString(),null);
    }
}
