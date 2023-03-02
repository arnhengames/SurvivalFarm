using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Active Level Map Creation Parameters")]
    [SerializeField] float spritePixelDimension = 32f;
    [SerializeField] int width = 60;
    [SerializeField] int length = 60;
    [SerializeField] int height = 1;

    float trueDimension;    //Multiplies integer represetation of Sprite Pixel Dimension by 0.01 to find accurate size
    float halfStep;         //Half of the true sprite dimension, used for isometric co-ordinates
    float quarterStep;      //Quarter of the true sprite dimension, used for isometric co-ordinates

    [Header("Level Nodes")]
    [SerializeField] List<LevelNode> nodes = new List<LevelNode>();

    int lastPlayerW;        //Last known position of player on Width Axis
    int lastPlayerL;        //Last known position of player on Length Axis

    int mapNodeWMin;        //Curent minimum extent of Width Axis on Active Level Map
    int mapNodeWMax;        //Curent maximum extent of Width Axis on Active Level Map
    int mapNodeLMin;        //Curent minimum extent of Length Axis on Active Level Map
    int mapNodeLMax;        //Curent maximum extent of Length Axis on Active Level Map

    void Start()
    {
        SetValues();        //Sets values used for isometric grid positioning based on Editor input

        lastPlayerW = PlayerInfo.widthLocation;     //Update last known location of Player
        lastPlayerL = PlayerInfo.lengthLocation;    //Update last known location of Player
    }

    private void Update()
    {
        //Check to see if our current player location has changed on the Width Axis
        int wDiff = PlayerInfo.widthLocation - lastPlayerW;
        lastPlayerW += wDiff;

        //If its changed, update the width of the Active Level Map
        if (wDiff != 0)
        {
            UpdateW(wDiff);
        }

        //Check to see if our current player location has changed on the Length Axis
        int lDiff = PlayerInfo.lengthLocation - lastPlayerL;
        lastPlayerL += lDiff;

        //If its changed, update the length of the Active Level Map
        if (lDiff != 0)
        {
            UpdateL(lDiff);
        }
    }

    void UpdateW(int diff)
    {
        if (diff > 0)
        {
            foreach (LevelNode node in nodes)
            {
                if ((node.widthIndex) == mapNodeWMin)
                {
                    node.UpdateWidthIndex(diff);
                }
            }

            mapNodeWMax++;
            mapNodeWMin++;
        } 
        else
        {
            foreach (LevelNode node in nodes)
            {
                if ((node.widthIndex) == mapNodeWMax)
                {
                    node.UpdateWidthIndex(diff);
                }
            }

            mapNodeWMax--;
            mapNodeWMin--;
        }

        UpdateBlockSortingOrder();
    }

    void UpdateL(int diff)
    {
        if (diff > 0)
        {
            foreach (LevelNode node in nodes)
            {
                if ((node.lengthIndex) == mapNodeLMin)
                {
                    node.UpdateLengthIndex(diff);
                }
            }

            mapNodeLMax++;
            mapNodeLMin++;
        }
        else
        {
            foreach (LevelNode node in nodes)
            {
                if ((node.lengthIndex) == mapNodeLMax)
                {
                    node.UpdateLengthIndex(diff);
                }
            }

            mapNodeLMax--;
            mapNodeLMin--;
        }

        UpdateBlockSortingOrder();
    }

    void UpdateBlockSortingOrder()
    {
        int sortingOrder = -32768;

        LevelNode[] allNodes = FindObjectsOfType<LevelNode>().OrderBy(o => o.nodeIndex).ToArray();

        foreach(LevelNode node in allNodes)
        {
            foreach (LevelBlock block in node.blocks)
            {
                block.spriteRenderer.sortingOrder = sortingOrder;
                sortingOrder++;
            }
        }
    }

    public void SetValues()
    {
        trueDimension = spritePixelDimension * 0.01f;
        halfStep = trueDimension / 2f;
        quarterStep = trueDimension / 4f;

        mapNodeWMin = -(width / 2);
        mapNodeWMax = (width / 2) - 1;
        mapNodeLMin = -(length / 2);
        mapNodeLMax = (length / 2) - 1;

        LevelInfo.levelWidth = width;
        LevelInfo.levelLength = length;
        LevelInfo.trueDimension = trueDimension;
        LevelInfo.halfStep = halfStep;
        LevelInfo.quarterStep = quarterStep;
    }

    public void CreateLevel()
    {
        nodes = new List<LevelNode>();

        GameObject activeLevelMap = GameObject.Find("Active Level Map");

        if (activeLevelMap != null)
        {
            GameObject.DestroyImmediate(activeLevelMap);
        }

        activeLevelMap = new GameObject("Active Level Map");
        activeLevelMap.transform.parent = transform;

        SetValues();

        int halfW = width / 2;
        int halfL = length / 2;

        for (int j = -halfL; j < halfL; j++)
        {
            for (int i = -halfW; i < halfW; i++)
            {
                GameObject parentNode = new GameObject("W: " + i.ToString() + ", L: " + j.ToString());
                parentNode.transform.parent = activeLevelMap.transform;

                LevelNode levelNode = parentNode.AddComponent<LevelNode>();
                levelNode.widthIndex = i;
                levelNode.lengthIndex = j;
                levelNode.UpdateNodeIndex();
                parentNode.name = levelNode.nodeIndex.ToString();

                for (int k = 0; k < height; k++)
                {
                    CreateBlock(TileTypes.basicBlock, levelNode, k, parentNode.transform);
                }

                nodes.Add(levelNode);
            }
        }

        UpdateBlockSortingOrder();
    }

    void CreateBlock(GameObject basicBlock, LevelNode node, int hIndex, Transform parent)
    {
        GameObject newObj = GameObject.Instantiate(basicBlock, LevelInfo.GridIndexToTransformPos(node.widthIndex, node.lengthIndex, hIndex), Quaternion.identity, parent);
        newObj.name = hIndex.ToString();

        LevelBlock newBlock = newObj.GetComponent<LevelBlock>();
        newBlock.myNode = node;

        newBlock.widthIndex = node.widthIndex;
        newBlock.lengthIndex = node.lengthIndex;
        newBlock.heightIndex = hIndex;

        node.blocks.Add(newBlock);
    }
}

public static class LevelInfo
{
    public static int levelWidth;
    public static int levelLength;

    public static float trueDimension;
    public static float halfStep;
    public static float quarterStep;

    public static Vector3 TransformPosToGridPos(Vector3 inputPos, int hIndex)
    {
        return new Vector3(inputPos.x,
                inputPos.y + (hIndex * quarterStep),
                0f);                                                                    //No change to zPosition
    }

    public static Vector3 GridIndexToTransformPos(int wIndex, int lIndex, int hIndex)
    {
        return new Vector3(
            wIndex * -halfStep + (lIndex * halfStep),                                   //Get xPosition
            wIndex * -quarterStep + (lIndex * -quarterStep) + (hIndex * halfStep),      //Get yPosition
            0f);                                                                        //No change to zPosition
    }

    public static Vector3 StepWidthAxis(Vector3 startPos, int steps)
    {
        return startPos + new Vector3(-halfStep * steps, -quarterStep * steps);
    }

    public static Vector3 StepLengthAxis(Vector3 startPos, int steps)
    {
        return startPos + new Vector3(halfStep * steps, -quarterStep * steps);
    }
}
