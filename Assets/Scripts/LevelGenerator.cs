using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Level Generator")]
class LevelGenerator : EditorTool
{
    static LevelManager levelManager;

    public static GameObject block;

    static int baseLayer = LayerMask.NameToLayer("BaseLayer");

    public static int width = 60;
    public static int length = 60;
    public static int height = 2;

    public static int maxDimensions = 20;

    static int sortOrder = -32767;
    static int oneNode = 14399;

    [MenuItem("My Tools/Create Level")]
    static void CreateLevel()
    {
        block = Resources.Load<GameObject>("BasicBlock");

        int nodesW = width / maxDimensions;
        int nodesL = length / maxDimensions;

        int remainderW = width - (nodesW * maxDimensions);
        int remainderL = length - (nodesL * maxDimensions);

        if (nodesW > 0) 
        { 
            width = maxDimensions; 
        }

        if (nodesL > 0) 
        { 
            length = maxDimensions; 
        }

        levelManager = FindObjectOfType<LevelManager>();
        levelManager.SetStepValues();

        for (int nodeW = 0; nodeW < nodesW; nodeW++)
        {
            for (int nodeL = 0; nodeL < nodesL; nodeL++)
            {
                GameObject parentNode = new GameObject();
                LevelNode levelNode = parentNode.AddComponent<LevelNode>();
                parentNode.name = ("Node: " + nodeW + ", " + nodeL);

                for (int k = 0; k < height; k++)
                {
                    for (int j = 0 + (nodeW * maxDimensions); j < width + (nodeW * maxDimensions); j++)
                    {
                        for (int i = 0 + (nodeL * maxDimensions); i < length + (nodeL * maxDimensions); i++)
                        {
                            if (k == 0)
                            {
                                CreateBlock(i, j, k, true, parentNode.transform, levelNode);
                            }
                            else
                            {
                                CreateBlock(i, j, k, false, parentNode.transform, levelNode);
                            }
                        }
                    }
                }
            }
        }
    }

   static void CreateBlock(int wIndex, int lIndex, int hIndex, bool baseNode, Transform parent, LevelNode node)
   {
        GameObject newObj = GameObject.Instantiate(block, FindObjectOfType<LevelManager>().GridIndexToTransformPos(wIndex, lIndex, hIndex), Quaternion.identity, parent);
        newObj.GetComponent<SpriteRenderer>().sortingOrder = GetBlockSortingOrder();
        newObj.name = GetBlockName(wIndex, lIndex, hIndex);

        LevelBlock newBlock = newObj.AddComponent<LevelBlock>();
        newBlock.widthIndex = wIndex;
        newBlock.lengthIndex = lIndex;
        newBlock.heightIndex = hIndex;

        node.blocks.Add(newBlock);

        if (baseNode)
        {
            PolygonCollider2D blockCollider = newBlock.AddComponent<PolygonCollider2D>();
            blockCollider.isTrigger = true;

            newObj.layer = baseLayer;
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
