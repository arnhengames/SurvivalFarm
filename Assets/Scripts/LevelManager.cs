using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] float spritePixelDimension = 32;
    float trueDimension;
    float halfStep;
    float quarterStep;

    void Start()
    {
        SetStepValues();
    }

    public Vector3 TransformPosToGridPos(Vector3 inputPos, int hIndex)
    {
        return new Vector3(inputPos.x,
                inputPos.y + (hIndex * quarterStep),
                0f);                                                                    //No change to zPosition
    }

    public Vector3 GridIndexToTransformPos(int wIndex, int lIndex, int hIndex)
    {                                                                  
        return new Vector3(
            wIndex * -halfStep + (lIndex * halfStep),                                   //Get xPosition
            wIndex * -quarterStep + (lIndex * -quarterStep) + (hIndex * quarterStep),   //Get yPosition
            0f);                                                                        //No change to zPosition
    }

    public Vector3 StepWidthAxis(Vector3 startPos, int steps)
    {
        return startPos + new Vector3(-halfStep * steps, -quarterStep * steps);
    }

    public Vector3 StepLengthAxis(Vector3 startPos, int steps)
    {
        return startPos + new Vector3(halfStep * steps, -quarterStep * steps);
    }

    public void SetStepValues()
    {
        trueDimension = spritePixelDimension * 0.01f;
        halfStep = trueDimension / 2f;
        quarterStep = trueDimension / 4f;
    }
}
