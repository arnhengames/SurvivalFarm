using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "EditorScene", menuName = "Editor Scene", order = 1)]
public class EditorScene : ScriptableObject
{
    int[,,] occupiedGridPositions;
    EditorBlock[] blocksInScene;

    int minWidth;
    int maxWidth;
    int minLength;
    int maxLength;
    int minHeight;
    int maxHeight;
    int levelMapWidth;
    int levelMapLength;
    int levelMapHeight;
    int offsetW;
    int offsetL;

    public void Initialize()
    {
        minWidth = int.MaxValue;
        maxWidth = int.MinValue;
        minLength = int.MaxValue;
        maxLength = int.MinValue;
        minHeight = int.MaxValue;
        maxHeight = int.MinValue;
    }

    public void UpdateScene(EditorBlock[] placedBlocks) 
    {
        blocksInScene = FindObjectsOfType<EditorBlock>();

        if (TestBlock(placedBlocks))
        {
            FindNewDimensions();
            SetTrueBlockPositions(blocksInScene);
        } 
        else
        {
            SetTrueBlockPositions(placedBlocks);
        }
    }

    void SetTrueBlockPositions(EditorBlock[] blocks)
    {
        foreach (EditorBlock block in blocks)
        {
            block.trueGridPosition = block.gridPosition.GetOffsetPosition(offsetW, offsetL);
            occupiedGridPositions[block.trueGridPosition.w, block.trueGridPosition.l, block.trueGridPosition.h] = 1;
        }
    }

    bool TestBlock(EditorBlock[] placedBlocks)
    {
        foreach (EditorBlock block in placedBlocks)
        {
            if (block.gridPosition.w < minWidth) return true;
            if (block.gridPosition.l < minLength) return true;
            if (block.gridPosition.h < minHeight) return true;
            if (block.gridPosition.w > maxWidth) return true;
            if (block.gridPosition.l > maxLength) return true;
            if (block.gridPosition.h > maxHeight) return true;
        }

        return false;
    }

    void MakeOutputGrid()
    {
        //Create Block Map
        occupiedGridPositions = new int[levelMapWidth, levelMapLength, levelMapHeight];

        foreach (EditorBlock block in blocksInScene)
        {
            occupiedGridPositions[block.trueGridPosition.w, block.trueGridPosition.l, block.trueGridPosition.h] = 1;
        }
    }

    void FindNewDimensions()
    {
        minWidth = int.MaxValue;
        maxWidth = int.MinValue;
        minLength = int.MaxValue;
        maxLength = int.MinValue;
        minHeight = int.MaxValue;
        maxHeight = int.MinValue;

        foreach (EditorBlock block in blocksInScene)
        {
            if (block.gridPosition.w < minWidth)
            {
                minWidth = block.gridPosition.w;
            }

            if (block.gridPosition.w > maxWidth)
            {
                maxWidth = block.gridPosition.w;
            }

            if (block.gridPosition.l < minLength)
            {
                minLength = block.gridPosition.l;
            }

            if (block.gridPosition.l > maxLength)
            {
                maxLength = block.gridPosition.l;
            }

            if (block.gridPosition.h < minHeight)
            {
                minHeight = block.gridPosition.h;
            }

            if (block.gridPosition.h > maxHeight)
            {
                maxHeight = block.gridPosition.h;
            }
        }

        levelMapWidth = Mathf.Abs(maxWidth - minWidth) + 1;
        levelMapLength = Mathf.Abs(maxLength - minLength) + 1;
        levelMapHeight = Mathf.Abs(maxHeight - minHeight) + 1;

        offsetW = minWidth < 0 ? Mathf.Abs(minWidth) : 0;
        offsetL = minLength < 0 ? Mathf.Abs(minLength) : 0;

        occupiedGridPositions = new int[levelMapWidth, levelMapLength, levelMapHeight];
    }

    void TestGrid()
    {
        for (int i = 0; i < levelMapWidth; i++)
        {
            for (int j = 0; j < levelMapLength; j++)
            {
                for (int k = 0; k < levelMapHeight; k++)
                {
                    Debug.Log(i.ToString() + ", " + j.ToString() + ", " + k.ToString() + ": " + occupiedGridPositions[i, j, k]);
                }
            }
        }
    }
}
