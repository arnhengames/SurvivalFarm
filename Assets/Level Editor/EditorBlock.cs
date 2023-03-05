using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditorBlock : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public GridPosition gridPosition;
    public GridPosition trueGridPosition;
    public float sortingIndex;

    public float GetBlockSortingIndex()
    {
        sortingIndex = 1000000 + (gridPosition.w + gridPosition.l) + (gridPosition.h * 5);
        return sortingIndex;
    }
}
