using DG.Tweening;
using System;
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

    [SerializeField] private CardColor _topCardColor;
    [SerializeField] private BoxCollider boxCol;
    [SerializeField] private Stack<Card> _selectedCard;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private RectTransform buyBtn;
    [SerializeField] private Transform anchor;
    [SerializeField] private Currency buyType;
    public int ID { get => id; set => id = value; }
    public float X { get => transform.position.x; }
    public float Y { get => transform.position.y; }
    public float Z { get => transform.position.z; }
    public Vector3 Pos { get { return new Vector3(X, Y, Z); } }

    public int FibIndex { get => fibIndex; set => fibIndex = value; }
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
        buyBtn.GetComponent<SlotBtn>().slotBtnClicked = new();
        buyBtn.GetComponent<SlotBtn>().slotBtnClicked.AddListener(IsSlotUnlocking);
        slotUnlocked = new();
        slotUnlocked.AddListener(SlotUnlocked);
    }



    private void OnDisable()
    {
        goldCollected.RemoveAllListeners();
        gemCollected.RemoveAllListeners();
    }
    public virtual void Start()
    {
        Init();
    }
    public void Init()
    {
        boxCol = GetComponentInChildren<BoxCollider>();
        _selectedCard = new();
        isEmpty = true;

        if (isDealer) dealer = transform.parent.GetComponent<Dealer>();

        if (_cards.Count == 0) return;
        CenterCollider();
        isEmpty = false;
        _topCardColor = _cards.Last().cardColor;
        if (_topCardColor != CardColor.Empty && !GameManager.instance.listCurrentCardColor.Contains(_topCardColor))
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
        buyBtn.gameObject.SetActive(isEnable);
        ScreenToWorld.Instance.SetWorldToCanvas(buyBtn);
        SwitchBtnType(isEnable, buyType);
    }
    internal void SwitchBtnType(bool isActive, Currency currencyType)
    {
        buyBtn.GetComponent<SlotBtn>().SetBtnType(isActive, currencyType);
    }

    internal void SetTargetToDealCard(bool b)
    {
        isDealBtnTarget = b;
    }
    public virtual void Update()
    {
        isEmpty = _cards.Count == 0;
        _topCardColor = isEmpty ? CardColor.Empty : _topCardColor;
        if (SlotCamera.instance.isScalingCamera)
        {
            ScreenToWorld.Instance.SetWorldToCanvas(buyBtn);
            buyBtn.transform.SetPositionAndRotation(anchor.position, Quaternion.identity);
            var scaleValue = SlotCamera.instance.scaleValue;
            Tween tween = buyBtn.DOScale(buyBtn.localScale - new Vector3(scaleValue, scaleValue, scaleValue), SlotCamera.instance.Mul_Time);
            tween.OnComplete(() => tween.Kill());
        }
    }

    internal CardColor TopColor()
    {
        return _topCardColor;
    }
    public void TapHandler()
    {
        if (status != SlotStatus.Active) return;
        //Debug.Log("TapHandle");
        List<Vector3> lastPos = new();
        if (Player.Instance.fromSlot is null && !isEmpty)
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
                //Debug.Log("PUSHING SELECTED CARD" + i);
                _selectedCard.Push(temStackSelected.Pop());
            }
            if (!changed)
            {
                _topCardColor = CardColor.Empty;
            }
        }
        else if (Player.Instance.fromSlot is not null && Player.Instance.fromSlot != this)
        {
            Slot toSlot = Player.Instance.toSlot = this;
            //IF SELECTED CARD PEEKD NOT SAME COLOR AS TO SLOT TOP CARD
            if (_topCardColor != Player.Instance.fromSlot._selectedCard.Peek().cardColor
                && _topCardColor != CardColor.Empty)
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
                UpdateSlotState();
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
            cardOffset = _cards.Count == 0 ? toSlot.GetPos().y + 0.1f : _cards.Last().transform.position.y + Player.Instance.cardPositionOffsetY;
            Player.Instance.isAnimPlaying = true;

            if (isDealer)
            {
                CardColor color = Player.Instance.fromSlot._selectedCard.Peek().cardColor;
                Color c = ConfigFileManager.Instance.ColorConfig.GetRecordByKeySearch(color).Color;
                dealer.fillImg.color = c;
            }
            float delay = 0;
            float z = _cards.Count == 0 ? toSlot.GetPos().z + 0.1f : _cards.Last().transform.position.z + Player.Instance.cardPositionOffsetZ; ;

            // Sent card to slot
            for (int i = 0; i < count; i++)
            {
                Card lastCard = Player.Instance.fromSlot._selectedCard.Pop();
                Player.Instance.fromSlot._cards.Remove(lastCard);
                lastCard.PlayAnimation(this, d, Player.Instance.height,
                        Player.Instance.ease, cardOffset, z, delay)
                    .OnComplete(() =>
                    {
                        if (isDealer)
                        {
                            //Debug.Log(dealer.fillImg.fillAmount);
                            dealer.fillImg.fillAmount += 0.1f;
                        }
                    });
                _cards.Add(lastCard);
                delay += Player.Instance.delay;
                cardOffset += Player.Instance.cardPositionOffsetY;
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
            Invoke(nameof(UpdateSlotState), 0.1f);
            return;

        }
    }
    public void UpdateSlotState()
    {
        //Debug.Log("Update slot state");   
        Player.Instance.isAnimPlaying = false;
        boxCol.enabled = true;
        _topCardColor = _cards.Last() == null ? CardColor.Empty : _cards.Last().cardColor;

        //custom this use with state machine
        if (isDealBtnTarget)
        {
            int diff = _cards.Count - 20;
            if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    GameObject time = _cards[i].gameObject;
                    time.SetActive(false);
                    SetColliderSize(-1);
                    //update collider size
                }
                _cards.RemoveRange(0, diff);
                float whY = transform.position.y;
                foreach (var c in _cards)
                {
                    c.transform.DOMoveY(whY, 0.1f);
                    whY = 0.01f;
                }
                //update collision center;
                isDealBtnTarget = false;
                CenterCollider();

            }
        }
        if (!isDealer) return;

        #region DealTable Update
        int count = _cards.Count;
        _cardCounter = count;

        if (count < sCounter) return;

        boxCol.enabled = false;
        int goldClaimed = dealer.RewardGold;
        int gemClaimed = dealer.RewardGem;

        Player.Instance.totalGold += goldClaimed;
        Player.Instance.totalGold += gemClaimed;

        float t = 0.05f;

        for (int i = 0; i < count; i++)
        {

            Invoke(nameof(SplashAndDisableCard), t);
            t += Player.Instance.timeDisableCard;
            exp++;
            Debug.Log($"exp {exp}");
        }
        DataAPIController.instance.AddGem(gemClaimed);
        DataAPIController.instance.AddGold(goldClaimed);
        goldCollected?.Invoke(goldClaimed);
        gemCollected?.Invoke(gemClaimed);
        expChanged?.Invoke(count);

        boxCol.enabled = true;
        Invoke(nameof(LevelUp), t + Player.Instance.timeDisableCard);
        #endregion
    }
    public bool CheckSlotIsInCamera()
    {
        SlotCamera cam = SlotCamera.instance;
        cam.GetCamera();
        //Debug.Log($"postion {transform.position} + left {CameraMain.instance.GetLeft()} " +
        //    $" + right {CameraMain.instance.GetRight()} + top {CameraMain.instance.GetTop()} + bot {CameraMain.instance.GetBottom()}");
        if (transform.position.x < cam.GetLeft()
            || transform.position.x > cam.GetRight()
                || transform.position.y > cam.GetTop()
                   || transform.position.y < cam.GetBottom() + 3f) return true;
        else return false;
    }
    public void EnableWhenInCamera()
    {
        if (!CheckSlotIsInCamera()) gameObject.SetActive(true);
        else gameObject.SetActive(false);
    }
    private void SplashAndDisableCard()
    {
        Card last = _cards.Last();
        //SplashVfx s = VFXPool.Instance.pool.SpawnNonGravity();
        //ParticleSystem splash = s.GetComponent<ParticleSystem>();
        //var mainVfx = splash.main;

        //mainVfx.startColor = VFXPool.Instance.GetColor(last.cardColor);
        //splash.gameObject.SetActive(true);
        if (_cards.Remove(last))
        {
            last.gameObject.SetActive(false);
            //VFXPool.Instance.PlayParticleAt(splash, last.transform.position);
            transform.parent.GetComponent<Dealer>().fillImg.fillAmount -= 0.1f;
            SetColliderSize(-1);
            CenterCollider();
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
        if (boxCol.center.z < 0) return;
        Vector3 size = boxCol.size;
        size = new Vector3(size.x, size.y, 0);
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
        Debug.Log("IS SLOT UNLOCKIN");
        if (isUnlocked)
        {
            Debug.Log("SLOT IS UNLOCKED");
            status = SlotStatus.Active;
            gameObject.SetActive(true);
            buyBtn.gameObject.SetActive(false);
            UpdateSlotStatus(status);
            SetSprite();
            UpdateSlotData();
            UpdateSlotConfig();
        }
        else return;
    }
    private void UpdateSlotData()
    {
        SlotData data = new();
        data.isUnlocked = true;
        Debug.Log("Save slot from data");
        DataAPIController.instance.SaveSlotData(id, data, (isDone) =>
        {
            if (!isDone) return;
            if (IsBackBoneSlot())
            {
                Debug.Log("Post new SLot data this " + Y);

                Vector3 camPos = SlotCamera.instance.GetCam().transform.position;
                SlotCamera.instance.targetPoint = new Vector3(0, camPos.y + 1.5f, camPos.z);
                SlotCamera.instance.ScaleByTimeCamera();
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
        if (temp.x == 0)
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
            configrecord.Status = status;
        }
    }
    private void IsSlotUnlocking(bool isUnlocking)
    {
        if (!isUnlocking)
        {
            Debug.Log($"IS UNLOCKING {isUnlocking.ToString().ToUpper()}");
            return;
        };

        int gold = DataAPIController.instance.GetGold();
        int gem = DataAPIController.instance.GetGem();

        if (buyType == Currency.Gold && unlockCost <= gold)
        {
            DataAPIController.instance.MinusGold(unlockCost, (isDone) =>
            {
                Debug.Log("MINUS GOLD DONE");
                if (isDone) slotUnlocked.Invoke(isDone);
            });
        }
        else if (buyType == Currency.Gem && unlockCost <= gem)
        {
            //TODO: change minus gold -> minus gem
            DataAPIController.instance.MinusGem(unlockCost, (isDone) =>
            {
                Debug.Log("MINUS GEM  DONE");
                slotUnlocked.Invoke(isDone);
                return;
            });

        }

    }


    public void SetSlotPrice(int id, int cost, Currency type)
    {
        if (this.id != id) return;
        unlockCost = cost;
        buyType = type;
        if (status == SlotStatus.Locked)
        {
            buyBtn.GetComponent<SlotBtn>().InitButton(unlockCost, buyType);
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
    public void LoadCardsFromData()
    {
        string slotKey = "Slot_" + ID.ToString() + "_Cards";
        string slotStatusKey = "Slot_" + ID.ToString() + "_Status";
        string slotFibIndexKey = "Slot_" + ID.ToString() + "_FibIndex";
        string slotUnlockCostKey = "Slot_" + ID.ToString() + "_UnlockCost";

        string cardData = PlayerPrefs.GetString(slotKey, string.Empty);
        //CardColor recc
        if (!string.IsNullOrEmpty(cardData))
        {
            var cardIds = cardData.Split(',').Select(id => int.Parse(id)).ToList();
            foreach (var cardId in cardIds)
            {
                // Assuming there's a method to get a Card by its ID.
                Card c = CardPool.Instance.pool.SpawnNonGravity();
                //c.ColorSetBy(colorRecord.Name, colorRecord.Color);
                //slot._cards.Add(card);
            }
        }

        //slot.status = (SlotStatus)PlayerPrefs.GetInt(slotStatusKey, (int)SlotStatus.Locked);
        //slot.FibIndex = PlayerPrefs.GetInt(slotFibIndexKey, 0);
        //slot.unlockCost = PlayerPrefs.GetInt(slotUnlockCostKey, 0);
    }

    void SaveCardListToData()
    {
        if (_cards.Count == 0) return;
        //remaining card save to player data slot;
        for (int i = 0; i <9; i++)
        {
            //string slotKey = "Slot_" + i.ToString() + "_Cards";
            //string slotStatusKey = "Slot_" + i.ToString() + "_Status";
            //string slotFibIndexKey = "Slot_" + i.ToString() + "_FibIndex";
            //string slotUnlockCostKey = "Slot_" + i.ToString() + "_UnlockCost";

            //var slot = GameManager.instance.SlotsList[i];
            //string cardData = string.Join(",", slot._cards.Select(c = &gt; c.GetComponent & lt; Card & gt; ().CardData()));
            //PlayerPrefs.SetString(slotKey, cardData);
            //PlayerPrefs.SetInt(slotStatusKey, (int)slot.status);
            //PlayerPrefs.SetInt(slotFibIndexKey, slot.FibIndex);
            //PlayerPrefs.SetInt(slotUnlockCostKey, slot.unlockCost);
        }

        PlayerPrefs.Save();
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
    private void OnApplicationQuit()
    {
        SaveCardListToData();

    }

}
