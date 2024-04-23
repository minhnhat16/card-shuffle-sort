using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class DealButton : MonoBehaviour
{
    public Transform spawnPoint;
    public const int spawnSize = 5;
    [SerializeField] private float delayBtwCards = 0.075f;  
    [SerializeField] private float delayBtwSlots = 0.25f;
    public void HandleTap()
    {
        Debug.Log("Handel tap dealbutton");
        if (Player.Instance.fromSlot is not null)
        {
            foreach (var card in Player.Instance.fromSlot.GetSelectedCards())
            {
                float tempY = card.transform.position.y;
                card.transform.DOMoveY(tempY + 0.1f, 0.2f);
            }
            Player.Instance.fromSlot.GetSelectedCards().Clear();
            Player.Instance.fromSlot.UpdateSlotState();


            Player.Instance.fromSlot = null;
            Player.Instance.toSlot = null;
        }
        Tween t = transform.DOScaleZ(0.5f, 0.24f).OnComplete(() =>
         {
             t = transform.DOScaleZ(1.25f, 1f);
             t.OnComplete(() => t.Kill());
         });

        float timer = 0.25f;
        foreach (var s in IngameController.instance.GetListSlotActive())
        {
            s.SetTargetToDealCard(true);
            StartCoroutine(SendingCard(s, timer));
            timer += delayBtwSlots;

        }
        Player.Instance.isDealBtnActive = false;
    }

    IEnumerator SendingCard(Slot s, float timer)
    {
        Debug.Log("Card is Sending");
        yield return new WaitForSeconds(timer);
            SendCardTo(s);

    }

    private void SendCardTo(Slot destination)
    {
        float d = Player.Instance.duration;
        float offset = destination._cards.Count == 0 ? destination.transform.position.y +0.1f : destination._cards.Last().transform.position.y + Player.Instance.cardPositionOffsetY;
        float z = destination._cards.Count == 0 ? Player.Instance.cardPositionOffsetZ : destination._cards.Last().transform.position.z + Player.Instance.cardPositionOffsetZ;

        //destination.SetCollisionEnable(false);

        Player.Instance.isAnimPlaying = true;

        CardColor targetColor = destination.TopColor();

        List<CardColor> option = new List<CardColor>(GameManager.instance.cardColors);

        option.Remove(targetColor);

        int randomIndex = UnityEngine.Random.Range(0, option.Count);

        Debug.Log($"random index {randomIndex}, option count {option.Count}");
        CardColor spawnColor = option[randomIndex];

        ColorConfigRecord colorRecord = IngameController.instance.colorConfig.GetRecordByKeySearch(spawnColor);
        float delay = 0;

        for (int i = 0; i < spawnSize; i++)
        {
            Card c = CardPool.Instance.pool.SpawnNonGravity();
            c.ColorSetBy(colorRecord.Name,colorRecord.Color);
            c.transform.SetLocalPositionAndRotation(spawnPoint.position, Quaternion.identity);
            c.PlayAnimation(destination, d, Player.Instance.height, Player.Instance.ease, offset,z , delay);
            destination._cards.Add(c);
            delay += delayBtwCards;

            offset += Player.Instance.cardPositionOffsetY;
            z += Player.Instance.cardPositionOffsetZ;
            //update collision size;
            destination.SetColliderSize(1);
        }
        destination.CenterCollider();
        StartCoroutine(UpdateSlotType(destination, delay + d));
    }

   IEnumerator  UpdateSlotType(Slot destination, float v)
    {
        yield return new WaitForSeconds(v);
        destination.UpdateSlotState();
    }
}
