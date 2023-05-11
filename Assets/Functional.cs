using System;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

internal class Functional
{
    //private static void ExecAndTempColor(Action drawFunc, Color color)
    //{
    //    if (!color.Equals(Gizmos.color))
    //    {
    //        Color prevColor = Gizmos.color;
    //        Gizmos.color = color;

    //        drawFunc();

    //        Gizmos.color = prevColor;
    //    }
    //    else
    //    {
    //        drawFunc();
    //    }
    //}

    public static void DrawWireCubeWithColor(Bounds bounds, Color color)
    {
        // https://answers.unity.com/questions/29797/how-to-get-8-vertices-from-bounds-properties.html

        Vector3 boundPoint1 = bounds.min;
        Vector3 boundPoint2 = bounds.max;
        Vector3 boundPoint3 = new(boundPoint1.x, boundPoint1.y, boundPoint2.z);
        Vector3 boundPoint4 = new(boundPoint1.x, boundPoint2.y, boundPoint1.z);
        Vector3 boundPoint5 = new(boundPoint2.x, boundPoint1.y, boundPoint1.z);
        Vector3 boundPoint6 = new(boundPoint1.x, boundPoint2.y, boundPoint2.z);
        Vector3 boundPoint7 = new(boundPoint2.x, boundPoint1.y, boundPoint2.z);
        Vector3 boundPoint8 = new(boundPoint2.x, boundPoint2.y, boundPoint1.z);

        // rectangular cuboid
        // top of rectangular cuboid (6-2-8-4)
        Debug.DrawLine(boundPoint6, boundPoint2, color);
        Debug.DrawLine(boundPoint2, boundPoint8, color);
        Debug.DrawLine(boundPoint8, boundPoint4, color);
        Debug.DrawLine(boundPoint4, boundPoint6, color);

        // bottom of rectangular cuboid (3-7-5-1)
        Debug.DrawLine(boundPoint3, boundPoint7, color);
        Debug.DrawLine(boundPoint7, boundPoint5, color);
        Debug.DrawLine(boundPoint5, boundPoint1, color);
        Debug.DrawLine(boundPoint1, boundPoint3, color);

        // legs (6-3, 2-7, 8-5, 4-1)
        Debug.DrawLine(boundPoint6, boundPoint3, color);
        Debug.DrawLine(boundPoint2, boundPoint7, color);
        Debug.DrawLine(boundPoint8, boundPoint5, color);
        Debug.DrawLine(boundPoint4, boundPoint1, color);
    }

    //public static void DrawWithColor(Action drawFunc, Color color)
    //{
    //    ExecAndTempColor(drawFunc, color);
    //}

    //public static void DrawWireCubeWithColor(Bounds objBounds, Color color)
    //{
    //    ExecAndTempColor(() => { Gizmos.DrawWireCube(objBounds.center, objBounds.size); }, color);
    //}

    //public static void DrawLineWithColor(Vector3 vec1, Vector3 vec2, Color color)
    //{
    //    ExecAndTempColor(() => { Debug.DrawLine(vec1, vec2, color); }, color);
    //}
}
