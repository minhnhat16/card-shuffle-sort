using System;
using UnityEngine;


[Serializable]
public class SlotConfigRecord
{
    [SerializeField]
    private int id;
    [SerializeField]
    private Vector3 pos;
    [SerializeField]
    private SlotStatus status;
 
    public int ID { get { return id; } }
    public Vector3 Pos { get { return pos; } }
    public SlotStatus Status { get { return status; } }
}
public class SlotConfig : BYDataTable<SlotConfigRecord>
{
    public override ConfigCompare<SlotConfigRecord> DefineConfigCompare()
    {
        configCompare = new ConfigCompare<SlotConfigRecord>("id");
        return configCompare;
    }
}
