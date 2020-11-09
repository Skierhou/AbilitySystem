using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public enum EMotionType
{
    MT_None,
    MT_Additive,
    MT_Override,
};

public class MotionClip
{
    public int priority;
    public float duration_Rotate;
    public float duration_Move;

    /* 固定方向距离位移 */
    public EMotionType motionType;
    public EDirectType directType;
    public Vector3 direction;
    public float distance;

    /* 旋转 */
    public EMotionType rotateType;
    public Vector3 torque;

    /* 旋转至固定点结束 */
    public bool bLookAt;
    public Quaternion startRot;
    public Quaternion endRot;

    /* 移动以及旋转曲线 */
    public AnimationCurve moveCurve;
    public AnimationCurve rotateCurve;

    public UnityAction OnMotionStart;
    public UnityAction OnMotionUpdate;
    public UnityAction OnMotionEnd;

    Vector3 velocity;
    Vector3 angularVelocity;

    Transform transform;
    float startTime_Rotate;
    float startTime_Move;

    public MotionClip(Transform transform)
    {
        this.transform = transform;
    }

    public void ApplyMotion(int priority, EMotionType motionType, EDirectType directType, float distance, float duration = 0.0f, AnimationCurve moveCurve = null)
    {
        if (this.priority < priority)
        {
            Reset();
            OnMotionStart?.Invoke();
        }
        startTime_Move = Time.time;
        this.priority = priority;
        this.motionType = motionType;
        this.directType = directType;
        this.direction = GetDirection(directType);
        this.distance = distance;
        this.duration_Move = duration;
        this.moveCurve = moveCurve;
    }
    public void ApplyRotate(int priority, EMotionType rotateType, EDirectType asixType, float rotateAngle, float duration = 0.0f, AnimationCurve rotateCurve = null)
    {
        if (this.priority < priority)
        {
            Reset();
            OnMotionStart?.Invoke();
        }
        startTime_Rotate = Time.time;
        this.priority = priority;
        this.rotateType = rotateType;
        this.torque = GetTorque(asixType, rotateAngle);
        this.duration_Rotate = duration;
        this.rotateCurve = rotateCurve;
    }

    //public void ApplyRotate(int priority, EMotionType rotateType, Quaternion startRot, EDirectType rotateAxis, float rotateAngle, float duration = 0.0f, AnimationCurve rotateCurve = null)
    //{
    //    bLookAt = true;
    //    this.priority = priority;
    //    this.rotateType = rotateType;
    //    this.startRot = startRot;
    //    this.duration = duration;
    //    this.rotateCurve = rotateCurve;
    //    this.torque = GetTorque(rotateAxis, rotateAngle);
    //    endRot = Quaternion.Euler(startRot.eulerAngles + GetTorque(rotateAxis, rotateAngle));
    //}

    public virtual void TickMotion(float inDeltaTime)
    {
        if (duration_Move > 0.0f && Time.time - startTime_Move <= duration_Move)
        {
            Update_Motion(inDeltaTime);
        }
        if (duration_Rotate > 0.0f && Time.time - startTime_Rotate <= duration_Rotate)
        {
            Update_Rotate(inDeltaTime);
        }
        else if (priority >= 0)
        {
            OnMotionEnd?.Invoke();
            Reset();
        }
    }

    public virtual void Reset()
    {
        bLookAt = false;
        priority = -1;
        directType = EDirectType.DT_WorldForward;
        duration_Move = 0.0f;
        duration_Rotate = 0.0f;
        velocity = Vector3.zero;
        angularVelocity = Vector3.zero;
        motionType = EMotionType.MT_Additive;
        rotateType = EMotionType.MT_Additive;
    }

    private void Update_Motion(float inDeltaTime)
    {
        float delta1, delta2;
        if (moveCurve is object)
        {
            delta1 = distance * moveCurve.Evaluate((Time.time - startTime_Move) / duration_Move);
            delta2 = distance * moveCurve.Evaluate((Time.time - startTime_Move + inDeltaTime) / duration_Move);
        }
        else
        {
            delta1 = Mathf.Lerp(0, distance, (Time.time - startTime_Move) / duration_Move);
            delta2 = Mathf.Lerp(0, distance, (Time.time - startTime_Move + inDeltaTime) / duration_Move);
        }
        switch (directType)
        {
            case EDirectType.DT_SelfForward:
                direction = transform.forward;
                break;
            case EDirectType.DT_SelfRight:
                direction = transform.right;
                break;
            case EDirectType.DT_SelfUp:
                direction = transform.up;
                break;
        }
        velocity = (delta2 - delta1) * direction / inDeltaTime;
    }
    private void Update_Rotate(float inDeltaTime)
    {
        //if (bLookAt)
        //{
        //    Vector3 v1, v2;
        //    if (moveCurve is object)
        //    {
        //        v1 = Quaternion.Lerp(startRot, endRot, rotateCurve.Evaluate((Time.time - startTime) / duration)).eulerAngles;
        //        v2 = Quaternion.Lerp(startRot, endRot, rotateCurve.Evaluate((Time.time - startTime + inDeltaTime) / duration)).eulerAngles;
        //    }
        //    else
        //    {
        //        v1 = Quaternion.Lerp(startRot, endRot, (Time.time - startTime) / duration).eulerAngles;
        //        v2 = Quaternion.Lerp(startRot, endRot, (Time.time - startTime + inDeltaTime) / duration).eulerAngles;
        //    }
        //    angularVelocity = (MathEx.ClampVector360(v2) - MathEx.ClampVector360(v1)) / inDeltaTime;
        //}
        //else
        //{
        //    angularVelocity = torque;
        //}
        angularVelocity = torque;
        if (moveCurve is object)
        {
            float rate = (Time.time - startTime_Rotate) / duration_Rotate;
            angularVelocity *= moveCurve.Evaluate(rate) / rate;
        }
    }
    Vector3 GetDirection(EDirectType directType)
    {
        switch (directType)
        {
            case EDirectType.DT_RelativeForward:
                return transform.forward;
            case EDirectType.DT_RelativeRight:
                return transform.right;
            case EDirectType.DT_RelativeUp:
                return transform.up;
            case EDirectType.DT_WorldForward:
                return Vector3.forward;
            case EDirectType.DT_WorldRight:
                return Vector3.right;
            case EDirectType.DT_WorldUp:
                return Vector3.up;
        }
        return Vector3.zero;
    }
    private Vector3 GetTorque(EDirectType asixType, float rotateAngle)
    {
        switch (asixType)
        {
            case EDirectType.DT_RelativeForward:
            case EDirectType.DT_SelfForward:
                return rotateAngle * transform.forward;
            case EDirectType.DT_RelativeRight:
            case EDirectType.DT_SelfRight:
                return rotateAngle * transform.right;
            case EDirectType.DT_RelativeUp:
            case EDirectType.DT_SelfUp:
                return rotateAngle * transform.up;
            case EDirectType.DT_WorldForward:
                return rotateAngle * Vector3.forward;
            case EDirectType.DT_WorldRight:
                return rotateAngle * Vector3.right;
            case EDirectType.DT_WorldUp:
                return rotateAngle * Vector3.up;
        }
        return Vector3.zero;
    }

    public virtual Vector3 GetVelocity()
    {
        return motionType == EMotionType.MT_None ? Vector3.zero : velocity;
    }
    public virtual Vector3 GetAngularVelocity()
    {
        return rotateType == EMotionType.MT_None ? Vector3.zero : angularVelocity;
    }
}