using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Slot : MonoBehaviour, IComparable<Slot>
{
    public List<Card> _cards; //Cards current in slot
    public SlotStatus status;

    private const float sizePerCard = 0.075f;
    [SerializeField] private bool isDealer;
    [SerializeField] private bool isEmpty;
    [SerializeField] private bool isDealBtnTarget;

    [SerializeField] private int id;
    [SerializeField] private int fibIndex;
    [SerializeField] private int unlockCost;
    [SerializeField] private float cardOffset;
    [SerializeField] private static int sCounter = 10;

    [SerializeField] private CardColorPallet _topCardColor;
    [SerializeField] private List<CardColorPallet> cardColorPallets;

    [SerializeField] private BoxCollider boxCol;
    [SerializeField] private Stack<Card> _selectedCard;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private RectTransform buyBtnRect;
    [SerializeField] private SlotBtn buyBtn;
    [SerializeField] private Transform anchor;
    [SerializeField] private Currency buyType;
    [SerializeField] private Vector3 scaleValue;

    [SerializeField] public UnityEvent<bool> onToucheHandle = new();
    public int ID { get => id; set => id = value; }
    public float X { get => transform.position.x; }
    public float Y { get => transform.position.y; }
    public float Z { get => transform.position.z; }
    public Vector3 Pos { get { return new Vector3(X, Y, Z); } }

    public int FibIndex { get => fibIndex; set => fibIndex = value; }
    public RectTransform BuyBtn { get => buyBtnRect; set => buyBtnRect = value; }
    public List<CardColorPallet> CardColorPallets { get => cardColorPallets; set => cardColorPallets = value; }
    public float CardOffset { get => cardOffset; set => cardOffset = value; }
    public int UnlockCost { get => unlockCost; set => unlockCost = value; }
    #region Dealer
    [SerializeField] private Dealer dealer;

    [SerializeField] private int slotLevel;
    [SerializeField] private int exp = 1;
    [SerializeField] private static int _cardCounter;
    #endregion

    #region UnityAction
    [SerializeField] public UnityEvent<float> expChanged;
    [HideInInspector] public UnityEvent<int> goldCollected = new();
    [HideInInspector] public UnityEvent<int> gemCollected = new();
    [HideInInspector] public UnityEvent<int> slotCanUnlock = new();
    [HideInInspector] public UnityEvent<bool> slotBtnClicked = new();
    [HideInInspector] public UnityEvent<bool> slotUnlocked = new();

    //[HideInInspector] public UnityEvent<>
    #endregion

    public Vector3 GetPos()
    {
        return transform.position;
    }
    private void OnEnable()
    {
        buyBtn.slotBtnClicked = new();
        buyBtn.slotBtnClicked.AddListener(IsSlotUnlocking);
        slotUnlocked = new();
        slotUnlocked.AddListener(SlotUnlocked);
        onToucheHandle.AddListener(TapHandler);
    }

    private void OnDisable()
    {
        onToucheHandle.RemoveListener(TapHandler);
        slotUnlocked.RemoveListener(SlotUnlocked);
        onToucheHandle.RemoveListener(TapHandler);
        _cards.Clear();
    }
    private void OnDestroy()
    {
        if(SlotCamera.Instance != null)
        {
            SlotCamera.Instance.OnScalingCamera -= HandleCameraScaling;

        }
        //SlotCamera.Instance.OnScalingCamera -= HandleCameraScaling;
    }
    public virtual void Start()
    {
        Init();
        SlotCamera.Instance.OnScalingCamera += HandleCameraScaling;
        //InvokeRepeating(nameof(), 0.1f, 0.167f);
    }
    public virtual void Update()
    {
        SlotUpdating();
    }
    public void Init()
    {
        boxCol = GetComponentInChildren<BoxCollider>();
        buyBtnRect.GetComponent<SlotBtn>().ParentAnchor = anchor;
        _selectedCard = new();
        isEmpty = true;
        if (status != SlotStatus.Active) boxCol.enabled = false;
        if (isDealer) dealer = transform.parent.GetComponent<Dealer>();

        if (_cards.Count == 0) return;
        CenterCollider();
        isEmpty = false;
        _topCardColor = _cards.Last().cardColor;
        if (_topCardColor != CardColorPallet.Empty && !GameManager.instance.listCurrentCardColor.Contains(_topCardColor))
        {
            GameManager.instance.listCurrentCardColor.Add(_topCardColor);
        }
    }
    public void SetSprite()
    {
        switch (status)
        {
            case SlotStatus.Active:

                spriteRenderer.sprite = SpriteLibControl.Instance.GetSpriteByName(status.ToString());
                break;
            case SlotStatus.InActive:
                SettingBuyBtn(false);
                spriteRenderer.sprite = SpriteLibControl.Instance.GetSpriteByName(status.ToString());
                break;
            case SlotStatus.Locked:
                SettingBuyBtn(true);
                spriteRenderer.sprite = SpriteLibControl.Instance.GetSpriteByName(status.ToString());
                break;
            default: break;
        }
    }
    public void SettingBuyBtn(bool isEnable)
    {
        int count = SlotCamera.Instance.mulCount;
        float scale = SlotCamera.Instance.ScaleValue[count];
        buyBtnRect.DOScale(new Vector3(scale, scale, scale), 0);
        buyBtnRect.gameObject.SetActive(isEnable);
        ScreenToWorld.Instance.SetWorldToCanvas(buyBtnRect);
        SwitchBtnType(isEnable, buyType);
    }
    internal void SwitchBtnType(bool isActive, Currency currencyType)
    {
        buyBtn.SetBtnType(isActive, currencyType);
    }
    public void UpdateCardPosition()
    {
        List<Card> temp = new(_cards);
        int c = _cards.Count();
        float offset = transform.position.y + 0.1f;
        for (int i = 0; i < c; i++)
        {
            Card tCard = temp[i];
            tCard.transform.DOMoveY(offset, 0.2f);
            offset += Player.Instance.cardPositionOffsetY;
            //lastPos.Add(tCard.transform.position);
        }
    }
    internal void SetTargetToDealCard(bool b)
    {
        isDealBtnTarget = b;
    }
    Tween tween;
 
    public void SlotUpdating()
    {

        bool isEmptyNow = _cards.Count == 0;
        if (status != SlotStatus.Active) return;
        if (isEmpty != isEmptyNow)
        {
            isEmpty = isEmptyNow;
            _topCardColor = isEmpty ? CardColorPallet.Empty : (_cards.Count > 0 ? _cards.Last().cardColor : CardColorPallet.Empty);
        }
    }
    internal void ReloadSlotButton()
    {
        //Debug.Log("Btn active" + buyBtn.gameObject.activeInHierarchy + "id " + id);
        ScreenToWorld.Instance.SetWorldToCanvas(buyBtnRect);
        buyBtnRect.transform.DOMove(anchor.position,0.075f);
        float a = SlotCamera.Instance.ScaleValue[SlotCamera.Instance.mulCount];
        scaleValue = new Vector3 (a, a, a);
        tween = buyBtnRect.DOScale(scaleValue, SlotCamera.Instance.Mul_Time);
        tween.OnComplete(() => tween.Kill());
    }
    internal CardColorPallet TopColor()
    {
        return _topCardColor;
    }
  
    internal void LoadCardData<T>(Stack<T> stackCardColor)
    {
        if (stackCardColor is null || stackCardColor.Count == 0) return;

        StartCoroutine(LoadCardDataCoroutine(stackCardColor));
    }

    private IEnumerator LoadCardDataCoroutine<T>(Stack<T> stackCardColor)
    {
        float delay = 0;
        float d = Player.Instance.duration;
        float offset = transform.position.y + 0.1f;
        float z = Player.Instance.cardPositionOffsetZ;
        CardType currentCardType = IngameController.instance.CurrentCardType;
         var colorConfig = ConfigFileManager.Instance.ColorConfig;
        while (stackCardColor.Count > 0)
        {
            Player.Instance.isAnimPlaying = true;
            T spawnColor = stackCardColor.Pop();
            ColorConfigRecord colorRecord = colorConfig.GetRecordByKeySearch(spawnColor);

            Card c = CardPool.Instance.pool.SpawnNonGravity();
            c.ColorSetBy(colorRecord.Name, currentCardType, colorRecord.Color);
            Vector3 worldPoint = new(0, -5, -10);
            c.transform.SetLocalPositionAndRotation(worldPoint, Quaternion.identity);
            c.PlayAnimation(this, d, Player.Instance.height, Player.Instance.ease, offset, z, delay);
            _cards.Add(c);
            CardColorPallets.Add(c.cardColor);
            delay += 0.075f;
            offset += Player.Instance.cardPositionOffsetY;
            z += Player.Instance.cardPositionOffsetZ;

            // Update collision size;
            SetColliderSize(1);
            _topCardColor = _cards.Last().cardColor;

            yield return new WaitForSeconds(0.075f); // Wait for 0.075 seconds before spawning the next card
            if(stackCardColor.Count <1) Player.Instance.isAnimPlaying = false;

        }

        StartCoroutine(UpdateSlotType(delay + d + 1f));
    }

    IEnumerator UpdateSlotType(float v)
    {
        yield return new WaitForSeconds(v);
        //yield return new WaitUntil(()=> _cards.Count)
        UpdateSlotState();
    }
    public void TapHandler(bool onTap)
    {
        if (status != SlotStatus.Active ||Player.Instance.isAnimPlaying) return;
        Debug.Log("TapHandle");
        List<Vector3> lastPos = new();
        if (Player.Instance.fromSlot == null && !isEmpty)
        {
            Player.Instance.fromSlot = this;
            Player.Instance.toSlot = null;

            int c = _cards.Count();
            List<Card> temp = new(_cards);
            Stack<Card> temStackSelected = new();

            temp.Reverse();
            bool changed = false;
            for (int i = 0; i < c; i++)
            {
                if (temp[i].cardColor == _topCardColor)
                {
                    Card tCard = temp[i];
                    temStackSelected.Push(tCard);
                    tCard.transform.DOMoveY(tCard.transform.position.y + 0.1f, 0.2f);
                    lastPos.Add(tCard.transform.position);
                }
                else
                {
                    _topCardColor = temp[i].cardColor;
                    changed = true;
                    break;
                }
            }
            int stackCount = temStackSelected.Count;
            for (int i = 0; i < stackCount; i++)
            {
                _selectedCard.Push(temStackSelected.Pop());
            }
            if (!changed)
            {
                _topCardColor = CardColorPallet.Empty;
            }
        }
        else if (Player.Instance.fromSlot != null && Player.Instance.fromSlot != this)
        {
            Slot toSlot = Player.Instance.toSlot = this;
            //IF SELECTED CARD PEEKD NOT SAME COLOR AS TO SLOT TOP CARD
            if (_topCardColor != Player.Instance.fromSlot._selectedCard.Peek().cardColor
                && _topCardColor != CardColorPallet.Empty && _cards.Count != 0)
            {
                foreach (var c in Player.Instance.fromSlot._selectedCard)
                {
                    float y = c.transform.position.y;
                    c.transform.DOMoveY(y - 0.1f, 0.2f);
                }
                CenterCollider();
                Player.Instance.fromSlot.CenterCollider();
                Player.Instance.fromSlot._selectedCard.Clear();
                Player.Instance.fromSlot.UpdateSlotState();
                //UpdateSlotState();
                Player.Instance.fromSlot.transform.DOShakePosition(
                          duration: 1f, // Duration of the shake
                          strength: new Vector3(0.25f, 0, 0), // Strength of the shake (0 on X and Z, 1 on Y)
                          vibrato: 10, // Vibrato (frequency of shakes)
                          randomness: 90, // Randomness (angle of shakes)
                          snapping: false, // Should the values be snapped to integer values
                          fadeOut: true, // Should the shake gradually fade out
                          randomnessMode: ShakeRandomnessMode.Harmonic
                          ); // The mode of randomness);
                SoundManager.instance.PlaySFX(SoundManager.SFX.MoveWrong);
                Player.Instance.fromSlot = null;
                Player.Instance.toSlot = null;
                return;
            }
            foreach (var c in Player.Instance.fromSlot._selectedCard)
            {
                Player.Instance.fromSlot._cards.Remove(c);

            }
            boxCol.enabled = false;
            float d = Player.Instance.duration;
            float count = Player.Instance.fromSlot._selectedCard.Count();
            CardOffset = _cards.Count == 0 ? toSlot.GetPos().y + 0.1f : _cards.Last().transform.position.y + Player.Instance.cardPositionOffsetY;
            Player.Instance.isAnimPlaying = false;

            if (isDealer)
            {
                Debug.Log("Is Dealer");
                CardColorPallet color = Player.Instance.fromSlot._selectedCard.Peek().cardColor;
                Color c = ConfigFileManager.Instance.ColorConfig.GetRecordByKeySearch(color).Color;
                dealer.fillImg.color = c;
            }
            float delay = 0;
            float z = _cards.Count == 0 ? toSlot.GetPos().z + 0.1f : _cards.Last().transform.position.z + Player.Instance.cardPositionOffsetZ; ;

            // Sent card to slot
            for (int i = 0; i < count; i++)
            {
                if (isDealer) dealer.SetRewardActive(false);
                Card lastCard = Player.Instance.fromSlot._selectedCard.Pop();
                Player.Instance.fromSlot._cards.Remove(lastCard);
                lastCard.PlayAnimation(this, d, Player.Instance.height,
                        Player.Instance.ease, CardOffset, z, delay)
                    .OnComplete(() =>
                    {
                        if (isDealer)
                        {
                            //Debug.Log(dealer.fillImg.fillAmount);
                            dealer.fillImg.fillAmount += 0.1f;
                        }
                    });
                _cards.Add(lastCard);
                CardColorPallets.Add(lastCard.cardColor);
                delay += Player.Instance.delay;
                CardOffset += Player.Instance.cardPositionOffsetY;
                z += Player.Instance.cardPositionOffsetZ;
                SetColliderSize(1);
                Player.Instance.fromSlot.SetColliderSize(-1);
            }
            Player.Instance.fromSlot = null;
            Player.Instance.toSlot = null;

            Invoke(nameof(UpdateSlotState), d + delay);
        }
        else if (Player.Instance.fromSlot is not null && Player.Instance.fromSlot == this)
        {
            foreach (var c in Player.Instance.fromSlot._selectedCard)
            {
                float y = c.transform.position.y;
                c.transform.DOMoveY(y - 0.1f, 0.2f);
            }
            Player.Instance.fromSlot._selectedCard.Clear();
            Player.Instance.fromSlot = null;
            Player.Instance.toSlot = null;
            Player.Instance.isAnimPlaying = false;
            Invoke(nameof(UpdateSlotState), 0.1f);
            return;

        }
    }
  
    public void UpdateSlotState()
    {
        // Enable box collider
        boxCol.enabled = true;

        // Update top card color
        var lastCard = _cards.LastOrDefault();
        _topCardColor = lastCard == null ? CardColorPallet.Empty : lastCard.cardColor;

        // Handle deal button target
        if (isDealBtnTarget)
        {
            int excessCards = _cards.Count - 20;
            if (excessCards > 0)
            {
                for (int i = 0; i < excessCards; i++)
                {
                    _cards[i].gameObject.SetActive(false);
                    SetColliderSize(-1);
                }

                _cards.RemoveRange(0, excessCards);
                CardColorPallets.RemoveRange(0, excessCards);

                float whY = transform.position.y;
                foreach (var card in _cards)
                {
                    card.transform.DOMoveY(whY, 0.1f);
                    whY += 0.01f;
                }

                CenterCollider();
                isDealBtnTarget = false;
                Player.Instance.isAnimPlaying = false;
            }
        }

        if (!isDealer) return;

        // Update deal table
        int cardCount = _cards.Count;
        _cardCounter = cardCount;

        if (cardCount < sCounter) return;

        boxCol.enabled = false;


        Player.Instance.totalGold += dealer.RewardGold;
        Player.Instance.totalGem += dealer.RewardGem;

        float delay = 0.05f;
        float totalDelay = delay * cardCount;

        for (int i = 0; i < cardCount; i++)
        {
            Invoke(nameof(SplashAndDisableCard), delay);
            delay += Player.Instance.timeDisableCard;
            exp++;
        }


        goldCollected.Invoke(dealer.RewardGold);
        gemCollected.Invoke(dealer.RewardGem);
        expChanged.Invoke(cardCount);

        boxCol.enabled = true;
        Player.Instance.isAnimPlaying = false;

        Invoke(nameof(LevelUp), totalDelay + Player.Instance.timeDisableCard);
    }

    public void SplashCardOnBomb(Action callback)
    {
        float t = 0.05f;
        int count = _cards.Count;
        for (int i = 0; i < count; i++)
        {
            if (i == count - 1) callback?.Invoke();
            Invoke(nameof(SplashAndDisableCardOnBomb), t);
            t += Player.Instance.timeDisableCard;
            exp++;
            //Debug.Log($"exp {exp}");
        }
        t += Player.Instance.timeDisableCard;
    }
    public bool CheckSlotIsInCamera()
    {
        //SlotCamera cam = SlotCamera.Instance;
        //cam.GetCamera();
        //Debug.Log($"postion {transform.position} + left {CameraMain.instance.GetLeft()} " +
        //    $" + right {CameraMain.instance.GetRight()} + top {CameraMain.instance.GetTop()} + bot {CameraMain.instance.GetBottom()}");
        if (transform.position.x < SlotCamera.Instance.GetLeft() - 1
            || transform.position.x > SlotCamera.Instance.GetRight() + 1
                || transform.position.y > SlotCamera.Instance.GetTop() - 2f
                   || transform.position.y < SlotCamera.Instance.GetBottom() + 3f) return false;
        else return true;
    }
    public void EnableWhenInCamera()
    {
        if (CheckSlotIsInCamera()) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
    private void SplashAndDisableCardOnBomb()
    {
        Card last = _cards.Last();
        if (_cards.Remove(last))
        {
            last.gameObject.SetActive(false);
            SetColliderSize(-1);
            CenterCollider();
        }
    }
    private void SplashAndDisableCard()
    {
        //Debug.LogWarning("SplashAndDisableCard");
        Card last = _cards.Last();
       

        if (_cards.Remove(last))
        {
            SplashVfx s = VFXPool.Instance.pool.SpawnNonGravity();

            s.SetPositionAndRotation(transform.position, Quaternion.identity);
            s.SetColorVFX(last.currentColor);
            s.PlayAndDeactivate();

            last.gameObject.SetActive(false);
            dealer.fillImg.fillAmount -= 0.1f;
            SetColliderSize(-1);
            CenterCollider();
            SoundManager.instance.PlaySFX(SoundManager.SFX.SPLASHCARD);

        }
    }
    public void SetColliderSize(float time)
    {
        boxCol.size += new Vector3(0, time * sizePerCard, 0);
        if (boxCol.size.z >= 0) return;
        Vector3 sz = boxCol.size;
        sz = new Vector3(sz.x, 0, sz.z);
        boxCol.size = sz;
    }
    public void CenterCollider()
    {
        Vector3 c = boxCol.center;
        boxCol.center = new Vector3(c.x, c.y, -boxCol.size.z / 2);
        if (boxCol.size.y<2.65) return;
        Vector3 size = boxCol.size;
        size = new Vector3(c.x, size.y/6, 0);
        boxCol.center = size;
       
    }
    private void LevelUp()
    {

        //TODO: ADD GOLD ANIM ON LEVEL UP
        //PlayCoin Collect Anim
        //Ivoke Collected coin
    }

    private void SlotUnlocked(bool isUnlocked)
    {
        //Debug.Log("IS SLOT UNLOCKIN");
        if (isUnlocked)
        {
            SoundManager.instance.PlaySFX(SoundManager.SFX.UnlockSlotSFX);
            //Debug.Log("SLOT IS UNLOCKED" + ID);
            status = SlotStatus.Active;
            gameObject.SetActive(true);
            buyBtnRect.gameObject.SetActive(false);
            boxCol.enabled = true;  
            UpdateSlotStatus(status);
            SetSprite();
            UpdateSlotData();
            UpdateSlotConfig();
            transform.DORotate(new Vector3(0, 180, 0), 0.5f, RotateMode.FastBeyond360);
            if (isDealer)
            {
                DealerData data = DataAPIController.instance.GetDealerData(dealer.Id);
                dealer.Status = status = data.status = SlotStatus.Active;
                dealer.SetRender();
                dealer.SetCurrencyAnimPosition();
                dealer.SetDealerAndFillActive(true);
                dealer.UpdateFillPostion();
                DataAPIController.instance.SetDealerToDictByID(dealer.Id, data, null);
            }
        }
        else return;
    }
    private void UpdateSlotData()
    {
        SlotData data = new();
        data.status = SlotStatus.Active;
        //Debug.Log("Save slot from data");
        onToucheHandle.AddListener(TapHandler);
        CardType type = IngameController.instance.CurrentCardType;
        DataAPIController.instance.SaveSlotData(id, data, type, (isDone) =>
        {
            if (!isDone) return;
            if (IsBackBoneSlot() && fibIndex < 12)
            {
                //Debug.Log("Post new SLot data this " + Y);

                Vector3 camPos = SlotCamera.Instance.GetCam().transform.position;
                SlotCamera.Instance.targetPoint = new Vector3(0, camPos.y + 1.5f, camPos.z);
                SlotCamera.Instance.ScaleByTimeCamera();
                IngameController.instance.SwitchNearbyCanUnlock(this);
            }
            else
            {
                IngameController.instance.SwitchNearbyCanUnlock(this);
            }
        });
    }
    public bool IsBackBoneSlot()
    {
        Vector3 temp = transform.position;
        if (temp.x == 0 && !isDealer)
        {
            return true;
        }
        return false;
    }
    internal void UpdateSlotConfig()
    {
        var configrecord = ConfigFileManager.Instance.SlotConfig.GetRecordByKeySearch(id);
        if (configrecord != null)
        {
            //configrecord.Status = status;
            SlotData newData = new();
            newData.status = status;
            newData.currentStack = new();
            CardType type = IngameController.instance.CurrentCardType;
            DataAPIController.instance.SaveSlotData(id, newData, type, (isDone) =>
            {
                //if (isDone) //Debug.Log("Add new slot data");
                //else Debug.Log("Save DataFail");
            });

        }
    }
    private void IsSlotUnlocking(bool isUnlocking)
    {
        if (!isUnlocking)
        {
            //Debug.Log($"IS UNLOCKING {isUnlocking.ToString().ToUpper()}");
            return;
        };

        int gold = DataAPIController.instance.GetGold();
        int gem = DataAPIController.instance.GetGem();

        if (buyType == Currency.Gold && unlockCost <= gold)
        {
            DataAPIController.instance.MinusGoldWallet(unlockCost, (isDone) =>
            {
                //Debug.Log("MINUS GOLD DONE");
                if (isDone) slotUnlocked.Invoke(isDone);
            });
        }
        else if (buyType == Currency.Gem && unlockCost <= gem)
        {
            //TODO: change minus gold -> minus gem
            DataAPIController.instance.MinusGemWallet(unlockCost, (isDone) =>
            {
                //Debug.Log("MINUS GEM  DONE");
                slotUnlocked.Invoke(isDone);
                return;
            });

        }

    }
    private void HandleCameraScaling()
    {
        ReloadSlotButton();
    }
    public void SetSlotPrice(int id, int cost, Currency type)
    {
        if (this.id != id) return;
        //Debug.Log($"Set Slot Price {id} cost {cost}");
        unlockCost = cost;
        buyType = type;
        if (status == SlotStatus.Locked)
        {
            buyBtn.InitButton(unlockCost, buyType);
        }
    }

    void FillAnim(float f)
    {
        if (f <= 0) return;
    }
    public Stack<Card> GetSelectedCards()
    {
        return _selectedCard;
    }
    bool CheckDataEmty()
    {
        //foreach slot have in data, check slot List<Card>
        return true;
    }
    public void UpdateSlotStatus(SlotStatus status)
    {
        this.status = status;
    }
    public bool CheckNeighborSlot(Slot checkSlot)
    {
        if (Pos.y == checkSlot.Y && (Pos.x + 2 == checkSlot.X || Pos.x - 2 == checkSlot.X))
        {
            return true;
        }
        else if ((Pos.y - 3 == checkSlot.Y || Pos.y + 3 == checkSlot.Y) && Pos.x == checkSlot.Pos.x)
        {
            //Debug.Log("Pos.y + 3 == checkSlot.Y && Pos.x == checkSlot.Pos.x true");
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SaveCardListToData()
    {
        if (_cards.Count == 0) return;
        //TODO: remaining card save to player data slot;
        Stack<CardColorPallet> stackColorData = new();
        for (int i = 0; i < _cards.Count; i++)
        {
            Card card = _cards[i];
            stackColorData.Push(card.cardColor);
        }
        var cardType = IngameController.instance.CurrentCardType;

        //int idData = isDealer == true ? id : id + 4;
        DataAPIController.instance.SaveStackCard(id, cardType, stackColorData);
    }



    public int CompareTo(Slot other)
    {
        if (other == null) return 1;
        return this.X.CompareTo(other.X);
    }
    internal void ReorderCards()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            float y = i * sizePerCard;
            _cards[i].transform.localPosition = new Vector3(0, y, 0);
        }
    }
    private void OndApplicationQuit()
    {
        SaveCardListToData();

    }

}
