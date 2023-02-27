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

    public static int width = 20;
    public static int length = 20;
    public static int height = 1;

    static int sortOrder = -15000;

    [MenuItem("My Tools/Create Level")]
    static void CreateLevel()
    {
        FindObjectOfType<LevelManager>().SetStepValues();

        block = Resources.Load<GameObject>("BasicBlock");

        for (int k = 0; k < height; k++)
        {
            LevelLayer newLayer = new LevelLayer();

            for (int j = 0; j < width; j++)
            {
                for (int i = 0; i < length; i++)
                {
                    GameObject newObj = GameObject.Instantiate(block, FindObjectOfType<LevelManager>().GridIndexToTransformPos(i, j, k), Quaternion.identity);
                    newObj.GetComponent<SpriteRenderer>().sortingOrder = GetBlockSortingOrder();
                    newObj.name = GetBlockName(i, j, k);

                    LevelBlock newBlock = newObj.AddComponent<LevelBlock>();
                    newBlock.widthIndex = i;
                    newBlock.lengthIndex = j;

                    newLayer.AddBlock(newBlock);
                }
            }

            FindObjectOfType<LevelManager>().AddLayer(newLayer);
        }
    }

    static string GetBlockName(int wIndex, int lIndex, int hIndex)
    {
        return ("Pos - W: " + wIndex + ", L: " + lIndex + ", H: " + hIndex);
    }

    static int GetBlockSortingOrder()
    {
        return sortOrder++;
    }
}
