using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IngameController : MonoBehaviour
{
    public static IngameController instance;
    List<Slot> _slot = new();
    public UnityEvent<int> onGoldChanged;
    [SerializeField]SlotConfig config;
    [SerializeField] public ColorConfig colorConfig;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        InitCardSlot();
    }
    protected internal void InitCardSlot()
    {
        var all = config.GetAllRecord();
        for (int i = 0; i < all.Count; i++)
        {
            Slot newSlot = SlotPool.Instance.pool.SpawnNonGravity();
            newSlot.ID = i;
            newSlot.transform.position = all[i].Pos;
            newSlot.status = all[i].Status;
        }
    }
    public List<Slot> GetListSlotActive()
    {
        if (SlotPool.Instance.pool.activeList.Count > 0)
        {
            _slot.Clear();
            for (int i = 0; i < SlotPool.Instance.pool.activeList.Count; i++)
            {
                Slot iSlot = SlotPool.Instance.pool.activeList[i];
                if (iSlot.status == SlotStatus.Active)
                {
                    _slot.Add(iSlot);
                }
             }
            return _slot;
        }

        return null;
    }
    public void SetSlotPositionByHand()
    {

    }
}
