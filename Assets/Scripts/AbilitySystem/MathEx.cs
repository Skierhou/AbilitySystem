using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct FTransformData
{
    public Vector3 Forward;
    public Vector3 Right;
    public Vector3 Up;
    public Vector3 Location;
    public FTransformData(Transform transform)
    {
        Forward = transform.forward;
        Right = transform.right;
        Up = transform.up;
        Location = transform.position;
    }
    public FTransformData(Vector3 inLoc,Vector3 inForward, Vector3 inRight, Vector3 inUp)
    {
        Location = inLoc;
        Forward = inForward;
        Right = inRight;
        Up = inUp;
    }
};

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

    /// <summary>
    /// 检测碰撞
    /// </summary>
    public static List<T> OverlapComponents<T>(EOverlapType inOverlapType, Vector3 inRange, FTransformData inTransData) where T : Component
    {
        List<Collider> colliders = OverlapColliders(inOverlapType, inRange, inTransData);
        List<T> res = new List<T>();
        foreach (Collider collider in colliders)
        {
            T temp = collider.GetComponent<T>();
            if (temp != null)
            {
                res.Add(temp);
            }
        }
        colliders.Clear();
        return res;
    }
    /// <summary>
    /// 检测碰撞
    /// </summary>
    public static List<Collider> OverlapColliders(EOverlapType inOverlapType, Vector3 inRange, FTransformData inTransData)
    {
        List<Collider> colliders = null;
        switch (inOverlapType)
        {
            case EOverlapType.Sphere:
                colliders = Physics.OverlapSphere(inTransData.Location, inRange.x).ToList();
                break;
            case EOverlapType.Box:
                inRange.z *= 0.5f;
                colliders = Physics.OverlapBox(inTransData.Location, inRange).ToList();
                break;
            case EOverlapType.Cylinder:
                colliders = Physics.OverlapCapsule(inTransData.Location, inTransData.Location + inRange.y * Vector3.up, inRange.x).ToList();
                break;
            case EOverlapType.Sector:
                Collider[] sectorTemps = Physics.OverlapSphere(inTransData.Location, inRange.x);
                colliders = new List<Collider>();
                foreach (Collider collider in sectorTemps)
                {
                    if (Vector3.Dot(inTransData.Forward, (collider.transform.position - inTransData.Location).normalized) > inRange.y * 0.5f * Mathf.Deg2Rad)
                    {
                        colliders.Add(collider);
                    }
                }
                break;
            case EOverlapType.Triangle:
                Collider[] triangleTemps = Physics.OverlapSphere(inTransData.Location, Pythagorean(inRange.x * 0.5f, inRange.y * 0.5f));
                // 0：left，1:right，2:top
                Vector3[] trianglePoints = GetTrianglePoints(inTransData, inRange);

                colliders = new List<Collider>();
                foreach (Collider collider in colliders)
                {
                    //三角形内满足2个条件：1.与前向量点乘>0；2.与后向量点乘<cos(夹角)
                    Vector3 dir1 = (collider.transform.position - inTransData.Location).normalized;
                    if (Vector3.Dot(inTransData.Forward, dir1) > 0)
                    {
                        Vector3 dir2 = (trianglePoints[0] - trianglePoints[2]).normalized;
                        Vector3 dir3 = (trianglePoints[1] - trianglePoints[2]).normalized;
                        float dot = Vector3.Dot(-inTransData.Forward, (collider.transform.position - trianglePoints[2]).normalized);
                        if (dot > 0 && dot < Vector3.Dot(dir2, dir3))
                        {
                            colliders.Add(collider);
                        }
                    }
                }
                break;
        }
        return colliders;
    }
    /// <summary>
    /// 计算一个三角形的三个顶点 0：left，1:right，2:top
    /// </summary>
    /// <param name="inRange">x:底边长，y:高</param>
    /// <returns>FMatrix3x3  0：left，1:right，2:top </returns>
    public static Vector3[] GetTrianglePoints(FTransformData inTransData, Vector3 inRange)
    {
        Vector3 tFwdHalfDir = inTransData.Forward * inRange.y * 0.5f;
        Vector3 tRightHalfDir = inTransData.Right * inRange.x * 0.5f;

        Vector3[] res = new Vector3[3];
        Vector3 left = inTransData.Location - tFwdHalfDir - tRightHalfDir;
        Vector3 right = inTransData.Location - tFwdHalfDir + tRightHalfDir;
        Vector3 top = inTransData.Location + tFwdHalfDir;

        res[0] = left;
        res[1] = right;
        res[2] = top;
        return res;
    }

    /// <summary>
    /// 勾股定理 x*x + y*y = z*z
    /// </summary>
    public static float Pythagorean(float x, float y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }
}