using System;
using UnityEngine;

internal class Functional
{
    private static void ExecAndTempColor(Action drawFunc, Color color)
    {
        if (!color.Equals(Gizmos.color))
        {
            Color prevColor = Gizmos.color;
            Gizmos.color = color;

            drawFunc();

            Gizmos.color = prevColor;
        }
        else
        {
            drawFunc();
        }
    }

    public static void DrawWithColor(Action drawFunc, Color color)
    {
        ExecAndTempColor(drawFunc, color);
    }

    public static void DrawWireCubeWithColor(Bounds objBounds, Color color)
    {
        ExecAndTempColor(() => { Gizmos.DrawWireCube(objBounds.center, objBounds.size); }, color);
    }

    public static void DrawLineWithColor(Vector3 vec1, Vector3 vec2, Color color)
    {
        ExecAndTempColor(() => { Gizmos.DrawLine(vec1, vec2); }, color);
    }
}
