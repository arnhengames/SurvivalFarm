using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Level Generator")]
class LevelGenerator : EditorTool
{
    public static GameObject block;

    public static int width = 10;
    public static int length = 10;
    public static int height = 2;

    static int sortOrder = -10000;

    [MenuItem("My Tools/Create Level")]
    static void CreateLevel()
    {
        block = Resources.Load<GameObject>("BasicBlock");

        for (int k = 0; k < height; k++)
        {
            GameObject newParent = new GameObject();
            newParent.name = ("Layer: " + k);

            Rigidbody2D newRB = newParent.AddComponent<Rigidbody2D>();
            newRB.bodyType = RigidbodyType2D.Static;

            CompositeCollider2D newCol = newParent.AddComponent<CompositeCollider2D>();
            newCol.vertexDistance = 0.01f;

            LevelLayer newLayer = newParent.AddComponent<LevelLayer>();
            newLayer.myLayer = k;

            for (int j = 0; j < width; j++)
            {
                for (int i = 0; i < length; i++)
                {
                    GameObject newObj = GameObject.Instantiate(block, GetIsometricPosition(i, j, k), Quaternion.identity, newParent.transform);
                    newObj.GetComponent<SpriteRenderer>().sortingOrder = GetBlockSortingOrder();
                    newObj.name = GetBlockName(i, j, k);

                    PolygonCollider2D polyCol = newObj.AddComponent<PolygonCollider2D>();
                    polyCol.usedByComposite = true;

                    LevelBlock newBlock = newObj.AddComponent<LevelBlock>();
                    newBlock.widthIndex = i;
                    newBlock.lengthIndex = j;
                    newBlock.heightIndex = k;
                }
            }
        }
    }

    static string GetBlockName(int wIndex, int lIndex, int hIndex)
    {
        return ("Pos - W: " + wIndex + ", L: " + lIndex + ", H: " + hIndex);
    }

    static Vector3 GetIsometricPosition(int wIndex, int lIndex, int hIndex)
    {
        return new Vector3(wIndex * -0.16f + (lIndex * 0.16f), wIndex * -0.08f + (lIndex * -0.08f) + (hIndex * 0.16f), 0f);
    }

    static int GetBlockSortingOrder()
    {
        return sortOrder++;
    }
}
