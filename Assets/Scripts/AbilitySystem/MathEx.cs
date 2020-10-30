using UnityEngine;

public static class MathEx
{
    public static Vector3 ClampVector360(Vector3 source)
    {
        source.x = Clamp360(source.x);
        source.y = Clamp360(source.y);
        source.z = Clamp360(source.z);
        return source;
    }
    public static float Clamp360(float x)
    {
        int n = (int)(x / 360.0f) + (x > 0 ? 0 : -1);
        x -= n * 360.0f;
        return x;
    }
}