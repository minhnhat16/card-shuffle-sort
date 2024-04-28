using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Slot : MonoBehaviour
{
    public List<Card> _cards; //Cards current in slot
    public SlotStatus status;

    private const float sizePerCard = 0.075f;
    [SerializeField] private bool isDealer;
    [SerializeField] private bool isEmpty;
    [SerializeField] private bool isDealBtnTarget;

    [SerializeField] private BoxCollider boxCol;
    [SerializeField] private CardColor _topCardColor;
    [SerializeField] private Stack<Card> _selectedCard;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Canvas renderCanvas;
    [SerializeField] private RectTransform buyBtn;
    [SerializeField] private GameObject anchor;
    [SerializeField] private float cardOffset;
    [SerializeField] private static int sCounter = 10;
    [SerializeField] private int id;
    [SerializeField] private int unlockCost;
    [SerializeField] private Currency buyType;
    public int ID { get => id; set => id = value; }
    #region Dealer
    [SerializeField] private Dealer dealer;

    [SerializeField] private int slotLevel;
    [SerializeField] private int exp = 1;
    [SerializeField] private static int _cardCounter;
    #endregion

    #region UnityAction
    [HideInInspector] public UnityEvent<int> goldCollected = new();
    [HideInInspector] public UnityEvent<int> gemCollected = new();
    [HideInInspector] public UnityEvent<int> slotCanUnlock = new();

    #endregion

    public Vector3 GetPos()
    {
        return transform.position;
    }
    private void OnEnable()
    {
        goldCollected.AddListener(null);
        gemCollected.AddListener(null);
       
        buyBtn.GetComponent<Button>().onClick.AddListener(UnlockSlot);
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
        if (_topCardColor != CardColor.Empty && !GameManager.instance.cardColors.Contains(_topCardColor))
        {
            GameManager.instance.cardColors.Add(_topCardColor);
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
                spriteRenderer.sprite = SpriteLibControl.Instance.GetSpriteByName(status.ToString());
                break;
            case SlotStatus.Locked:
                SettingBuyBtn();
                spriteRenderer.sprite = SpriteLibControl.Instance.GetSpriteByName(status.ToString());
                break;
            default: break;
        }

    }
    public void SettingBuyBtn()
    {
        buyBtn.gameObject.SetActive(true);
        Debug.Log($"Rect transform {buyBtn.anchoredPosition}");
        ScreenToWorld.Instance.SetWorldToCanvas(buyBtn);
        SwitchBtnType(buyType);
    }
    internal void SwitchBtnType(Currency currencyType)
    {
        buyBtn.GetComponent<SlotBtn>().SetBtnType(currencyType);
    }
    internal void SetTargetToDealCard(bool b)
    {
        isDealBtnTarget = b;
    }

    public virtual void Update()
    {
        isEmpty = _cards.Count == 0;
        _topCardColor = isEmpty ? CardColor.Empty : _topCardColor;
    }

    internal CardColor TopColor()
    {
        return _topCardColor;
    }

    public void TapHandler()
    {
        if (status != SlotStatus.Active) return;
        Debug.Log("TapHandle");
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
                //Color c = ConfigFileManager.Instance.ColorConfig.GetRecordByKeySearch(color).Color;
                Color c = IngameController.instance.colorConfig.GetRecordByKeySearch(color).Color;
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
                            Debug.Log(dealer.fillImg.fillAmount);
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
        // Save slot gold + gem by config
        Player.Instance.totalGold += (1 + slotLevel) * 100 / 2;
        if (slotLevel > 5)
        {
            Player.Instance.totalGem += slotLevel - 4;
        }
        float t = 0.05f;

        for (int i = 0; i < count; i++)
        {

            Invoke(nameof(SplashAndDisableCard), t);
            t += Player.Instance.timeDisableCard;
            exp++;
        }
        boxCol.enabled = true;
        Invoke(nameof(LevelUp), t + Player.Instance.timeDisableCard);
        #endregion
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
        sz  = new Vector3(sz.x, 0, sz.z);
        boxCol.size = sz;
    }
    public void CenterCollider()
    {
        Vector3 c = boxCol.center;
        boxCol.center = new Vector3(c.x, c.y, -boxCol.size.z / 2);
        if (boxCol.center.z <0) return;
        Vector3 size = boxCol.size;
        size = new Vector3(size.x, size.y, 0);
        boxCol.center = size;
    }
    private void LevelUp()
    {
        //PlayCoin Collect Anim
        //Ivoke Collected coin
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
    void SaveCardListToData()
    {
        if (_cards.Count == 0) return;
        //remaining card save to player data slot;
    }
    public void UnlockSlot()
    {
        Debug.Log("Unlock slot");
    }
    private void OnApplicationQuit()
    {
        SaveCardListToData();
    }

}
