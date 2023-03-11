using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public struct GridPosition
{
    public int w;
    public int l;
    public int h;

    public GridPosition(int width, int length)
    {
        w = width;
        l = length;
        h = int.MinValue;
    }

    public GridPosition(int width, int length, int height)
    {
        w = width;
        l = length;
        h = height;
    }

    public GridPosition(GridPosition inputGridPos, int height)
    {
        w = inputGridPos.w;
        l = inputGridPos.l;
        h = height;
    }

    public Vector3 GetTransformPosition()
    {
        return LevelData.GridIndexToTransformPos(this);
    }

    public GridPosition GetOffsetPosition(int offsetW, int offsetL)
    {
        return new GridPosition(w + offsetW, l + offsetL, h);
    }

    public void AssignValues(GridPosition input)
    {
        w = input.w;
        l = input.l;
        h = input.h;
    }

    public void AssignValues(GridPosition input, int inputH)
    {
        w = input.w;
        l = input.l;
        h = inputH;
    }

    public void AssignValues(int inputW, int inputL)
    {
        w = inputW;
        l = inputL;
        h = int.MinValue;
    }

    public void AssignValues(int inputW, int inputL, int inputH)
    {
        w = inputW;
        l = inputL;
        h = inputH;
    }

    public string GetValueString()
    {
        if (h != int.MinValue)
        {
            return w.ToString() + ", " + l.ToString() + ", " + h.ToString();
        } 
        else
        {
            return w.ToString() + ", " + l.ToString();
        }
    }

    public static bool IsWithin(GridPosition toTest, int minW, int maxW, int minL, int maxL)
    {
        if ((toTest.w >= minW) && (toTest.w <= maxW) && (toTest.l >= minL) && (toTest.l <= maxL))
        {
            return true;
        } 
        else
        {
            return false;
        }
    }

    public static GridPosition[] GetArrayBetween(GridPosition startPoint, GridPosition endPoint)
    {
        List<GridPosition> output = new List<GridPosition>();

        int startW = Mathf.Min(startPoint.w, endPoint.w);
        int endW = Mathf.Max(startPoint.w, endPoint.w);
        int startL = Mathf.Min(startPoint.l, endPoint.l);
        int endL = Mathf.Max(startPoint.l, endPoint.l);
        int startH = Mathf.Min(startPoint.h, endPoint.h);
        int endH = Mathf.Max(startPoint.h, endPoint.h);

        for (int i = startW; i <= endW; i++)
        {
            for (int j = startL; j <= endL; j++)
            {
                for (int k = startH; k <= endH; k++)
                {
                    GridPosition gridIndex = new GridPosition(i, j, k);

                    output.Add(gridIndex);
                }
            }
        }

        return output.ToArray();
    }
}
