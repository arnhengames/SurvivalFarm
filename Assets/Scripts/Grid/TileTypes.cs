using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileSprites
{
    public static Sprite[] grassRidge = Resources.LoadAll<Sprite>("Sprites/ridgetest");
}

public class TileType
{
    public GridPosition gridPosition;
    public Sprite sprite;
}


