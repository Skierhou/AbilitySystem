using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum EMotionType
{
    MT_Additive,
    MT_Override,
};

public class MotionClip
{
    public EMotionType motionType;
    public EMotionType rotateType;

    public virtual void OnStart()
    {
    }
    public virtual void TickMotion()
    {
    }
    public virtual Vector3 GetVelocity()
    {
        return Vector3.zero;
    }
    public virtual Vector3 GetRotateRate()
    {
        return Vector3.zero;
    }
}