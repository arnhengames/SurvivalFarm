using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileTypes
{
    public static GameObject basicBlock = Resources.Load<GameObject>("Prefabs/BasicBlock");

    public static Sprite grassSprite = Resources.Load<Sprite>("Sprites/GrassBlock");
    public static Sprite stoneSprite = Resources.Load<Sprite>("Sprites/StoneBlock");
    public static Sprite snowSprite = Resources.Load<Sprite>("Sprites/SnowBlock");
}
