using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Slot : MonoBehaviour
{
    public List<Card> _cards; //Cards current in slot
    public SlotStatus status;

    [SerializeField] private bool isDealer;
    [SerializeField] private bool isEmpty;
    [SerializeField] private bool isDealBtnTarget;

    [SerializeField] private BoxCollider2D boxCol;
    [SerializeField] private CardColor _topCardColor;
    [SerializeField] private Stack<Card> _selectedCard;

    [SerializeField] private float cardOffset;
    [SerializeField] private static int sCounter = 10;
    [SerializeField] private int id;
    public int ID { get => id; set => id = value; }
    #region Dealer
    [SerializeField] private Dealer dealer;

    [SerializeField] private int slotLevel;
    [SerializeField] private static int _cardCounter;
    #endregion

    #region UnityAction
    [HideInInspector] public UnityEvent<int> goldCollected = new();
    [HideInInspector] public UnityEvent<int> gemCollected = new();
    #endregion

    private void OnEnable()
    {
        goldCollected.AddListener(null);
        gemCollected.AddListener(null);

    }
    private void OnDisable()
    {
        goldCollected.RemoveAllListeners();
        gemCollected.RemoveAllListeners();
    }
    public virtual void Start()
    {
        boxCol = GetComponentInChildren<BoxCollider2D>();
        _selectedCard = new();
        isEmpty = true;

        if (isDealer) dealer = transform.parent.GetComponent<Dealer>();

        if (_cards.Count == 0) return;

        isEmpty = false;
        _topCardColor = _cards.Last().cardColor;
        if (_topCardColor != CardColor.Empty && !GameManager.instance.cardColors.Contains(_topCardColor))
        {
            GameManager.instance.cardColors.Add(_topCardColor);
        }
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
                _topCardColor = CardColor.Empty;
            }
        }
        else if (Player.Instance.fromSlot is not null && Player.Instance.fromSlot != this)
        {
            Player.Instance.toSlot = this;
            //IF SELECTED CARD PEEKD NOT SAME COLOR AS TO SLOT TOP CARD
            if (_topCardColor != Player.Instance.fromSlot._selectedCard.Peek().cardColor
                && _topCardColor != CardColor.Empty)
            {
                Debug.Log($"if top card color != selected card {_topCardColor != Player.Instance.fromSlot._selectedCard.Peek().cardColor && _topCardColor != CardColor.Empty}");
                foreach (var c in Player.Instance.fromSlot._selectedCard)
                {
                    float y = c.transform.position.y;
                    Debug.Log($"float y {y}");
                    c.transform.DOMoveY(y, 0.2f);
                }

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

            cardOffset = _cards.Count == 0 ? 0.1f : _cards.Last().transform.position.y + Player.Instance.cardPositionOffsetY;
            Debug.Log($"Card offset {cardOffset}");
            Player.Instance.isAnimPlaying = true;

            if (isDealer)
            {
                dealer.fillImg.color = VFXPool.Instance.GetColor(Player.Instance.fromSlot._selectedCard.Peek().cardColor);

            }

            float delay = 0;
            // Sent card to slot
            for(int i = 0; i < count; i++)
            {
                Card lastCard = Player.Instance.fromSlot._selectedCard.Pop();
                Player.Instance.fromSlot._cards.Remove(lastCard);
                lastCard.PlayAnimation(Player.Instance.toSlot, d, Player.Instance.height,
                        Player.Instance.ease, cardOffset, delay)
                    .OnComplete(() =>
                    {
                        if (isDealer)
                        {
                            dealer.fillImg.fillAmount += 0.1f;
                        }
                    });
                _cards.Add(lastCard);
                delay += Player.Instance.delay;
                cardOffset += Player.Instance.cardPositionOffsetY;
            }
            Player.Instance.fromSlot = null;
            Player.Instance.toSlot = null;

            Invoke(nameof(UpdateSlotState), d + delay);
        }

    }

    public void UpdateSlotState()
    {
        Debug.Log("Update slot state");
        Player.Instance.isAnimPlaying = false;
        boxCol.enabled = true;
        _topCardColor = _cards.Last().cardColor;

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
                    //update collider size
                }
                _cards.RemoveRange(0, diff);
                float whY = 0;
                foreach (var c in _cards)
                {
                    c.transform.DOMoveY(whY, 0.1f);
                    whY = 0.01f;
                }
                //update collision center;
                isDealBtnTarget = false;
            }
        }
        if (!isDealer) return;

        #region DealTable Update
        int count = _cards.Count;
        _cardCounter = count;

        if (count < sCounter) return;

        boxCol.enabled = false;
        // Save slot gold + gem by config
        Player.Instance.totalGold += count * 30;
        if (slotLevel > 5)
        {
            Player.Instance.totalGem += slotLevel - 4;
        }
        float t = 0;

        for (int i = 0; i < count; i++)
        {

            //Invoke(nameof(SplashAndDisableCard), t);
            t += Player.Instance.timeDisableCard;
        }
        Invoke(nameof(LevelUp), t + Player.Instance.timeDisableCard);
        #endregion
        Debug.Log($"box collider {boxCol.isActiveAndEnabled}");
    }
    private void SplashAndDisableCard()
    {
        Card last = _cards.Last();
        SplashVfx s = VFXPool.Instance.pool.SpawnNonGravity();
        ParticleSystem splash = s.GetComponent<ParticleSystem>();
        var mainVfx = splash.main;

        mainVfx.startColor = VFXPool.Instance.GetColor(last.cardColor);
        splash.gameObject.SetActive(true);
        if (_cards.Remove(last))
        {
            last.gameObject.SetActive(false);
            VFXPool.Instance.PlayParticleAt(splash, last.transform.position);
            transform.parent.GetComponent<Dealer>().fillImg.fillAmount -= 0.1f;
        }
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

    }
    private void OnApplicationQuit()
    {
        SaveCardListToData();
    }

}
