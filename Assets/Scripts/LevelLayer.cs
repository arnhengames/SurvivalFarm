using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLayer : MonoBehaviour
{
    public int myLayer = 0;
    public CompositeCollider2D myCollider;
    LevelBlock[] myBlocks;

    private void Start()
    {
        myCollider = GetComponent<CompositeCollider2D>();
        myBlocks = GetComponentsInChildren<LevelBlock>();
    }

    public void DisableColliders()
    {
        for (int i = 0; i < myBlocks.Length; i++)
        {
            myBlocks[i].myCollider.enabled = false;
        }
    }

    public void EnableColliders()
    {
        for (int i = 0; i < myBlocks.Length; i++)
        {
            myBlocks[i].myCollider.enabled = true;
        }
    }

    private void Update()
    {
        //Debug.Log("Layer " + myLayer + ": Collider is set to " + myCollider.isActiveAndEnabled);
    }
}
