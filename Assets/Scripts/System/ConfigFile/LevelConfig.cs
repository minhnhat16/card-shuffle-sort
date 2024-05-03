using System;
using UnityEngine;

[Serializable]
public class LevelConfigRecord 
{
    [SerializeField]
    private int id; // Level ID
    [SerializeField]
    private int experience; //Expearience for levelup
    [SerializeField]
    private bool isComplete;

    public int Id { get { return id; } }
    public int Experience { get { return experience; } }
    public bool IsComplete { get { return isComplete; } }

}

public class LevelConfig : BYDataTable<LevelConfigRecord>
{
    public override ConfigCompare<LevelConfigRecord> DefineConfigCompare()
    {
        configCompare = new ConfigCompare<LevelConfigRecord>("id");
        return configCompare;
    }
}
