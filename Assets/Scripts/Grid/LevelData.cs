using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public static int[,,] sceneMap;

    static float halfStep = 0.32f;
    static float quarterStep = 0.16f;

    public static void InitializeSceneMap(int sizeWidth, int sizeLength, int sizeHeight)
    {
        sceneMap = new int[sizeWidth, sizeLength, sizeHeight];
    }

    public static Vector3 GridIndexToTransformPos(int wIndex, int lIndex, int hIndex)
    {
        return new Vector3(
            wIndex * -halfStep + (lIndex * halfStep),                                   //Get xPosition
            wIndex * -quarterStep + (lIndex * -quarterStep) + (hIndex * halfStep),      //Get yPosition
            0f);                                                                        //No change to zPosition
    }

    public static Vector3 GridIndexToTransformPos(GridPosition gridPosition)
    {
        return new Vector3(
            gridPosition.w * -halfStep + (gridPosition.l * halfStep),                                           //Get xPosition
            gridPosition.w * -quarterStep + (gridPosition.l * -quarterStep) + (gridPosition.h * halfStep),      //Get yPosition
            0f);                                                                                                //No change to zPosition
    }

    public static Vector3 GridIndexToTransformPos(int[] gridPos)
    {
        return new Vector3(
            gridPos[0] * -halfStep + (gridPos[1] * halfStep),                                       //Get xPosition
            gridPos[0] * -quarterStep + (gridPos[1] * -quarterStep) + (gridPos[2] * halfStep),      //Get yPosition
            0f);                                                                                    //No change to zPosition
    }

    public static Vector3 StepWidthAxis(Vector3 startPos, int steps)
    {
        return startPos + new Vector3(-halfStep * steps, -quarterStep * steps);
    }

    public static Vector3 StepLengthAxis(Vector3 startPos, int steps)
    {
        return startPos + new Vector3(halfStep * steps, -quarterStep * steps);
    }

    public static Vector3 StepHeightAxis(Vector3 inputPos, int steps)
    {
        return new Vector3(inputPos.x,
                inputPos.y + (steps * quarterStep),
                0f);
    }

    public static GridPosition TransformPosToGridPos(Vector3 position)
    {
        GridPosition output = new GridPosition(
            -Mathf.FloorToInt(((position.x / halfStep) + (position.y / quarterStep)) * 0.5f),
            -Mathf.FloorToInt(((position.y / quarterStep) - (position.x / halfStep)) * 0.5f)
        );

        return output;
    }
}