using UnityEngine;

public static class ExtensionMethods
{
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
}