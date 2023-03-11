using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorBlock : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public GameObject[] outline;
    public bool hasOutline = false;

    public void CheckOutline()
    {

    }

    public void TurnOff()
    {
        spriteRenderer.sprite = null;
        transform.position = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void Initialize(Sprite sprite, GridPosition gridPosition)
    {
        spriteRenderer.sprite = sprite;
        transform.position = LevelData.GridIndexToTransformPos(gridPosition);
        float sort = 100000 + (gridPosition.w + gridPosition.l) + (gridPosition.h * 5);
        transform.position = new Vector3(transform.position.x, transform.position.y, sort);
    }
}
