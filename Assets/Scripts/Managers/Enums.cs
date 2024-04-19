using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Left,
    Right,
    Up,
    Down,
    UpperLeft,
    UpperRight,
    DownLeft,
    DownRight,
    Unknown
}
public enum ItemType
{
   Magnet = 0,
   Bomb = 1,
}
public enum Currency
{
   Gold = 0,
   Gem = 1,
}
public enum IEDailyType
{
    Available = 0,
    Unavailable = 1,
    Claimed = 2,
}
public enum CardType
{
    Diamond,
    Hexagon,
    Shirtm,
    Default,
}
public enum CardColor
{
    Red ,
    Yellow,
    Blue,
    Green,
    Black,
    Empty
}

public enum SlotStatus
{
    Active,
    InActive,
    Locked,
}
