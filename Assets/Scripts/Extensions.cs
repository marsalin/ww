using UnityEngine;

public static class Extensions
{
    public static Vector3 IgnoreZ(this Vector3 v, float overwriteValue = 0)
    {
        return new Vector3(v.x, v.y, overwriteValue);
    }
    public static Vector3 IgnoreY(this Vector3 v, float overwriteValue = 0)
    {
        return new Vector3(v.x, overwriteValue, v.z);
    }
}
