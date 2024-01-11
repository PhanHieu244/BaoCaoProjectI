using System;
using System.Collections.Generic;
using UnityEngine;

public static class ArrangeStrategy
{
    public static List<Vector2> GetRadiusArrange(int amountPoint, float radius, 
        float deltaPos, float maxRangeAngle = 60f, int maxPosInCircle = 3)
    {
        var coordinateList = new List<Vector2>();
        var anglesOddIndex = GetAnglesOddIndex(maxRangeAngle, maxPosInCircle);
        var anglesEvenIndex = GetAnglesEvenIndex(maxRangeAngle, maxPosInCircle);
        var maxPyramidSize = (maxPosInCircle + 1) * maxPosInCircle / 2;
        var anglesMaxIndex = maxPosInCircle % 2 == 0 ? anglesEvenIndex : anglesOddIndex;
        int rowAmount = 0;
        List<int> totalArrangement = new List<int>();
        while (amountPoint > maxPyramidSize)
        {
            foreach (var angleMaxIndex in anglesMaxIndex)
            {
                coordinateList.Add(GetCoordinate(angleMaxIndex, radius));
            }
            amountPoint -= anglesMaxIndex.Count;
            totalArrangement.Add(anglesMaxIndex.Count);
            rowAmount++;
        }

        var arrangement = Arrangement(amountPoint, maxPosInCircle);
        foreach (var amount in arrangement)
        {
            DevLog.Log("amount" ,amount);
            var anglesIndex = amount % 2 == 0 ? anglesEvenIndex : anglesOddIndex;
            for (int i = 0; i < amount; i++)
            {
                coordinateList.Add(GetCoordinate(anglesIndex[i], radius));
            }

            rowAmount++;
        }
        totalArrangement.AddRange(arrangement);
        var startPos = GetYStartPos(rowAmount, deltaPos);
        int index = 0;
        for (var rowID = 0; rowID < totalArrangement.Count; rowID++)
        {
            for (int i = 0; i < totalArrangement[rowID]; i++)
            {
                coordinateList[index] = new Vector2( coordinateList[index].x,
                    coordinateList[index].y + (startPos - rowID * deltaPos));
                index++;
            }
        }

        return coordinateList;
    }

    private static List<int> Arrangement(int amount, int maxIndexInCircle)
    {
        var listArrangement = new List<int>();
        if (amount <= maxIndexInCircle)
        {
            listArrangement.Add(amount);
            return listArrangement;
        }
        while (amount > 0)
        {
            var count = (amount + 1) / 2;
            if (count > maxIndexInCircle)
            {
                listArrangement.Add(maxIndexInCircle);
                amount -= maxIndexInCircle;
                continue;
            }
            listArrangement.Add(count);
            listArrangement.Add(amount - count);
            break;
        }
        return listArrangement;
    }

    private static float DegreesToRadians(float degrees)
    {
        return (float) (degrees * Math.PI / 180.0f);
    }
    
    private static Vector2 GetCoordinate (float angleInDegrees, float radius)
    {
        var angleInRadians = DegreesToRadians(angleInDegrees);
        // Calculate the coordinates
        var x = (float)(radius * Math.Sin(angleInRadians));
        var y = (float)(radius * Math.Cos(angleInRadians));
        return new Vector2(x, y);
    }

    private static float GetYStartPos(int rowAmount, float deltaPos)
    {
        var height = (rowAmount - 1) * deltaPos;
        return height / 2f;
    }

    private static List<float> GetAnglesOddIndex(float maxAngle, int maxPosInCircle)
    {
        var listAngle = new List<float>();
        var amountPosSide =  (maxPosInCircle - 1) / 2; //amount pos in left or right
        maxPosInCircle /= 2;
        maxPosInCircle *= 2;
        listAngle.Add(0);
        var deltaAngle = maxAngle / maxPosInCircle;
        for (int i = 1; i <= amountPosSide; i++)
        {
            listAngle.Add(deltaAngle * i);
            listAngle.Add(-deltaAngle * i);
        }

        return listAngle;
    }

    private static List<float> GetAnglesEvenIndex(float maxAngle, int maxPosInCircle)
    {
        var listAngle = new List<float>();
        var amountPosSide =  maxPosInCircle / 2; //amount pos in left or right
        maxPosInCircle = amountPosSide * 2;
        var deltaAngle = maxAngle / maxPosInCircle;
        var deltaAngleStart = deltaAngle / 2;
        listAngle.Add(deltaAngleStart);
        listAngle.Add(-deltaAngleStart);
        for (int i = 1; i < amountPosSide; i++)
        {
            listAngle.Add( deltaAngleStart + deltaAngle * i);
            listAngle.Add(-( deltaAngleStart + deltaAngle * i));
        }

        return listAngle;
    }
}