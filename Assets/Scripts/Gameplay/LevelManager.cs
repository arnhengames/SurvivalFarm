using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Active Level Map Creation Parameters")]
    [SerializeField] int width = 60;
    [SerializeField] int length = 60;
    [SerializeField] int height = 1;

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
        lastPlayerW = PlayerInfo.widthLocation;     //Update last known location of Player
        lastPlayerL = PlayerInfo.lengthLocation;    //Update last known location of Player

        mapNodeWMin = -(width / 2);        
        mapNodeWMax = width / 2;        
        mapNodeLMin = -(length / 2);        
        mapNodeLMax = length / 2;        
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
                    node.UpdateWidthIndex(diff, width, length);
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
                    node.UpdateWidthIndex(diff, width, length);
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
                    node.UpdateLengthIndex(diff, width, length);
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
                    node.UpdateLengthIndex(diff, width, length);
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

    public void CreateActiveLevelMap()
    {
        nodes = new List<LevelNode>();

        GameObject basicBlock = Resources.Load<GameObject>("Prefabs/BasicBlock");
        GameObject activeLevelMap = GameObject.Find("Active Level Map");

        if (activeLevelMap != null)
        {
            GameObject.DestroyImmediate(activeLevelMap);
        }

        activeLevelMap = new GameObject("Active Level Map");
        activeLevelMap.transform.parent = transform;

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
                levelNode.UpdateNodeIndex(length);
                parentNode.name = levelNode.nodeIndex.ToString();

                for (int k = 0; k < height; k++)
                {
                    CreateBlock(basicBlock, levelNode, k, parentNode.transform);
                }

                nodes.Add(levelNode);
            }
        }

        UpdateBlockSortingOrder();
    }

    void CreateBlock(GameObject basicBlock, LevelNode node, int hIndex, Transform parent)
    {
        GameObject newObj = GameObject.Instantiate(basicBlock, LevelData.GridIndexToTransformPos(node.widthIndex, node.lengthIndex, hIndex), Quaternion.identity, parent);
        newObj.name = hIndex.ToString();

        LevelBlock newBlock = newObj.GetComponent<LevelBlock>();
        newBlock.myNode = node;

        newBlock.widthIndex = node.widthIndex;
        newBlock.lengthIndex = node.lengthIndex;
        newBlock.heightIndex = hIndex;

        node.blocks.Add(newBlock);
    }
}
