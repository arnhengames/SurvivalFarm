using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LevelNode : MonoBehaviour
{
    public List<LevelBlock> blocks = new List<LevelBlock>();

    public int widthIndex;
    public int lengthIndex;

    public int nodeIndex;

    public void UpdateWidthIndex(int diff, int levelWidth, int levelLength)
    {
        if (diff > 0)
        {
            transform.position = LevelData.StepWidthAxis(transform.position, levelWidth);
            widthIndex = widthIndex + levelWidth;
        }
        else
        {
            transform.position = LevelData.StepWidthAxis(transform.position, -levelWidth);
            widthIndex = widthIndex - levelWidth;
        }

        foreach (LevelBlock block in blocks)
        {
            block.UpdateBlockIndices();
        }

        UpdateNodeIndex(levelLength);
    }

    public void UpdateLengthIndex(int diff, int levelWidth, int levelLength)
    {
        if (diff > 0)
        {
            transform.position = LevelData.StepLengthAxis(transform.position, levelLength);
            lengthIndex = lengthIndex + levelLength;
        }
        else
        {
            transform.position = LevelData.StepLengthAxis(transform.position, -levelLength);
            lengthIndex = lengthIndex - levelLength;
        }

        foreach (LevelBlock block in blocks)
        {
            block.UpdateBlockIndices();
        }

        UpdateNodeIndex(levelLength);
    }

    public void UpdateNodeIndex(int levelLength)
    {
        nodeIndex = (widthIndex + (lengthIndex * levelLength));
    }
}
