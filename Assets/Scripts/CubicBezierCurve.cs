using UnityEngine;

// This class implements cubic Bezier curves -- not linear or quadratic.
public class CubicBezierCurve
{
    private Vector3[] controlVerts;

    public CubicBezierCurve(Vector3[] cvs) => SetControlVerts(cvs);

    public void SetControlVerts(Vector3[] cvs)
    {
        // Cubic Bezier curves require 4 cvs.
        //Debug.Log($"length equals to 4: {cvs.Length == 4} ");
        controlVerts = cvs;
    }

    /// <summary>
    /// Get the point on the curve for a particular t value
    /// </summary>
    /// <param name="t"> </param>
    /// <returns> </returns>
    public Vector3 GetPoint(float t)
    {
        //Debug.Log($"t belongs to [0,1]: {(t >= 0.0f) && (t <= 1.0f)}");
        float c = 1.0f - t;

        // The Bernstein polynomials.
        float bb0 = c * c * c;
        float bb1 = 3 * t * c * c;
        float bb2 = 3 * t * t * c;
        float bb3 = t * t * t;

        Vector3 point = controlVerts[0] * bb0 + controlVerts[1] * bb1 + controlVerts[2] * bb2 + controlVerts[3] * bb3;
        return point;
    }

    /// <summary>
    /// Get the vector of tangent for a particular t value.
    /// </summary>
    /// <param name="t"> t = [1,0] </param>
    /// <returns> </returns>
    public Vector3 GetTangent(float t)
    {
        //Debug.Log($"t belongs to [0,1]: {(t >= 0.0f) && (t <= 1.0f)}");

        Vector3 q0 = controlVerts[0] + ((controlVerts[1] - controlVerts[0]) * t);
        Vector3 q1 = controlVerts[1] + ((controlVerts[2] - controlVerts[1]) * t);
        Vector3 q2 = controlVerts[2] + ((controlVerts[3] - controlVerts[2]) * t);

        Vector3 r0 = q0 + ((q1 - q0) * t);
        Vector3 r1 = q1 + ((q2 - q1) * t);
        Vector3 tangent = r1 - r0;

        return tangent;
    }
}