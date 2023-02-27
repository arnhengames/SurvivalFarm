using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLayer : MonoBehaviour
{
    public List<LevelBlock> blocks = new List<LevelBlock>();  

    public void AddBlock(LevelBlock block)
    {
        blocks.Add(block);
    }

    public LevelBlock GetBlock(int widthIndex, int lengthIndex)
    {
        foreach (LevelBlock block in blocks)
        {
            if (block.widthIndex == widthIndex && block.lengthIndex == lengthIndex)
            {
                return block;
            }
        }

        return null;
    }
}
