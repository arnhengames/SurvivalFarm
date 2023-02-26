using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBlock : MonoBehaviour
{
    public int widthIndex;
    public int lengthIndex;
    public int heightIndex;

    public PolygonCollider2D myCollider;

    private void Start()
    {
        myCollider = GetComponent<PolygonCollider2D>();
    }
}
