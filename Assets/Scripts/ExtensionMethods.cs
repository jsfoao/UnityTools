using UnityEngine;

public static class JMath
{
    public const float TAU = 6.28318530718f;

    public static Vector3 Round(this Vector3 v, float decimals = 1f)
    {
        v.x = Mathf.Round(v.x * decimals) / decimals;
        v.y = Mathf.Round(v.y * decimals) / decimals;
        v.z = Mathf.Round(v.z * decimals) / decimals;
        return v;
    }

    public static float Round(this float v, float decimals)
    {
        return Mathf.Round(v / decimals) * decimals;
    }

    public static Vector2 GetUnitVectorByAngle(float angRad)
    {
        return new Vector2
        (
            Mathf.Cos(angRad),
            Mathf.Sin(angRad)
        );
    }
}

public static class JGizmos
{

    public static void DrawWireCircle(Vector3 pos, Quaternion rot, float radius, int detail = 32)
    {
        Vector3[] points3D = new Vector3[detail];

        for (int i = 0; i < detail; i++)
        {
            float t = i / (float) detail;
            float angRad = t * JMath.TAU;

            // point in the unit circle
            Vector2 point2D = JMath.GetUnitVectorByAngle(angRad) * radius;
            points3D[i] = pos + rot * point2D;
        }

        for (int i = 0; i < detail - 1; i++)
        {
            Gizmos.DrawLine(points3D[i], points3D[i + 1]);
        }
        Gizmos.DrawLine(points3D[detail - 1], points3D[0]);
    }

}