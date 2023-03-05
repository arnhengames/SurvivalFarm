using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBlock : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public LevelNode myNode;

    public int widthIndex;
    public int lengthIndex;
    public int heightIndex;

    public void UpdateBlockIndices()
    {
        widthIndex = myNode.widthIndex;
        lengthIndex = myNode.lengthIndex;
    }

    void UpdateBlockSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
