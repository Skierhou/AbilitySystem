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

    Vector3 velocity;
    Vector3 rotate;

    public MotionClip(Vector3 motion)
    {
        velocity = motion;
    }

    public virtual void OnStart()
    {

    }
    public virtual void TickMotion()
    {

    }
    public virtual Vector3 GetVelocity()
    {
        return velocity;
    }
    public virtual Vector3 GetRotateRate()
    {
        return rotate;
    }
}