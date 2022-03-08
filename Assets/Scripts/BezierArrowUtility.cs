using UnityEngine;

public static class BezierArrowUtility
{
    public static void SetBezierArrow(ArrowSO arrowSO, CubicBezierCurve spline)
        => SetBezierArrow(arrowSO.Nodes, spline);

    private static void SetBezierArrow(
        RectTransform[] bezierNodes,
        CubicBezierCurve spline)
    {
        float t;
        for (int i = 0; i < bezierNodes.Length; i++)
        {
            t = i / (bezierNodes.Length - 1f);

            bezierNodes[i].position = spline.GetPoint(t);
            bezierNodes[i].eulerAngles = new Vector3(0, 0, VectorToAngle(spline.GetTangent(t)));
        }
    }

    private static float VectorToAngle(Vector2 angle)
        => Vector2.SignedAngle(Vector2.up, angle);
}