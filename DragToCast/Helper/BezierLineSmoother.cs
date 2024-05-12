using UnityEngine;

namespace DragToCast.Helper;

internal static class BezierLineSmoother
{
    internal static Vector3 BezierQuadratic(this Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, float time)
    {
        var u = 1 - time;
        var tt = time * time;
        var uu = u * u;

        var p = uu * startPoint;
        p += 2 * u * time * controlPoint;
        p += tt * endPoint;

        return p;
    }

    internal static Vector3 BezierCubic(this Vector3 startPoint, Vector3 firstControlPoint, Vector3 secondControlPoint, Vector3 endPoint, float time)
    {
        var u = 1 - time;
        var tt = time * time;
        var ttt = tt * time;
        var uu = u * u;
        var uuu = uu * u;

        var p = uuu * startPoint;
        p += 3 * uu * time * firstControlPoint;
        p += 3 * u * tt * secondControlPoint;
        p += ttt * endPoint;

        return p;
    }
}
