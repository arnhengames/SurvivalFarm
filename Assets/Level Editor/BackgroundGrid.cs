using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGrid : MonoBehaviour
{
    public GameObject backgroundGrid1;
    public GameObject backgroundGrid2;

    public Transform top;
    public Transform left;
    public Transform right;

    // Start is called before the first frame update
    void Start()
    {
        LevelEditor levelEditor = FindObjectOfType<LevelEditor>();

        int xDimension = levelEditor.startingWidth;
        int yDimension = levelEditor.startingLength;

        left.transform.position = LevelData.StepWidthAxis(left.transform.position, xDimension);
        right.transform.position = LevelData.StepLengthAxis(right.transform.position, yDimension);

        for (int i = 0; i <= xDimension; i++)
        {
            for (int j = 0; j <= yDimension; j++)
            {
                if ((i + j) % 2 == 0)
                {
                    Instantiate(backgroundGrid1, LevelData.GridIndexToTransformPos(i, j, 0), Quaternion.identity, transform);
                } else
                {
                    Instantiate(backgroundGrid2, LevelData.GridIndexToTransformPos(i, j, 0), Quaternion.identity, transform);
                }
            }
        }
    }
}
