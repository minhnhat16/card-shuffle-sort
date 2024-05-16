using System;
using UnityEngine;


[Serializable]
public class SlotConfigRecord
{
    [SerializeField]
    private int id;
    [SerializeField]
    private int fibIndex;
    [SerializeField]
    private float x;
    [SerializeField]
    private float y;
    [SerializeField]
    private float z ;
    [SerializeField]
    private SlotStatus status;
    public int ID { get { return id; } }
    public float X { get => x; }
    public float Y { get => y; }
    public float Z { get => z; }
    public SlotStatus Status { get { return status; } set { status = value; } }
    public Vector3 Pos { get => new Vector3(x, y, z); }
    public int FibIndex { get => fibIndex; }
}
public class SlotConfig : BYDataTable<SlotConfigRecord>
{
    public override ConfigCompare<SlotConfigRecord> DefineConfigCompare()
    {
        configCompare = new ConfigCompare<SlotConfigRecord>("id");
        return configCompare;
    }
}
