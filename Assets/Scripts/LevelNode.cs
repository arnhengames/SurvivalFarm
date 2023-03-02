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

    public void UpdateWidthIndex(int diff)
    {
        if (diff > 0)
        {
            transform.position = LevelInfo.StepWidthAxis(transform.position, LevelInfo.levelWidth);
            widthIndex = widthIndex + LevelInfo.levelWidth;
        }
        else
        {
            transform.position = LevelInfo.StepWidthAxis(transform.position, -LevelInfo.levelWidth);
            widthIndex = widthIndex - LevelInfo.levelWidth;
        }

        foreach (LevelBlock block in blocks)
        {
            block.UpdateBlockIndices();
        }

        UpdateNodeIndex();
    }

    public void UpdateLengthIndex(int diff)
    {
        if (diff > 0)
        {
            transform.position = LevelInfo.StepLengthAxis(transform.position, LevelInfo.levelLength);
            lengthIndex = lengthIndex + LevelInfo.levelLength;
        }
        else
        {
            transform.position = LevelInfo.StepLengthAxis(transform.position, -LevelInfo.levelLength);
            lengthIndex = lengthIndex - LevelInfo.levelLength;
        }

        foreach (LevelBlock block in blocks)
        {
            block.UpdateBlockIndices();
        }

        UpdateNodeIndex();
    }

    public void UpdateNodeIndex()
    {
        nodeIndex = (widthIndex + (lengthIndex * LevelInfo.levelLength));
    }
}
