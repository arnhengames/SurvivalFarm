using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public LevelLayer[] layers;
    int layerIgnoreCollision;
    int layerCollision;
    int playerLayer;

    private void Start()
    {
        layers = FindObjectsOfType<LevelLayer>();
    }

    public void ChangeCollisionLayer(int newCollisionLayer)
    {
        Debug.Log(newCollisionLayer);

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].myLayer == newCollisionLayer)
            {
                layers[i].EnableColliders();
            } 
            else
            {
                layers[i].DisableColliders();
            }
        }
    }
}
