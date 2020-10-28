using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ELifeType
{
    LT_Start,
    LT_Loop,
    LT_End,
};

public class MovementComponent : MonoBehaviour
{
    public Controller Controller { get; protected set; }
    public CharacterController CharacterController { get; protected set; }

    //Motion
    public MotionClip MotionClip { get; protected set; }
    public RootMotionClip RootMotionClip { get; protected set; }

    public Vector3 Gravity { get; protected set; }
    public Vector3 Velocity { get; protected set; }
    public Vector3 AngularVelocity { get; protected set; }

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
    }

    public virtual void OnPossess(Controller controller)
    {
        Controller = controller;
        MotionClip = new MotionClip(transform);
        RootMotionClip = new RootMotionClip(GetComponentInChildren<Animator>());
    }
    public virtual void OnUnPossess()
    {
        Controller = null;
    }

    protected void FixedUpdate()
    {
        if (MotionClip is object)
            MotionClip.TickMotion(Time.fixedDeltaTime);

        // 计算重力
        if (CharacterController.isGrounded)
            Gravity = Vector3.zero;
        else
            Gravity += Physics.gravity * Time.fixedDeltaTime;

        Velocity = CalculateVeloctiy();
        AngularVelocity = CalculateAngularVelocity();

        CharacterController.SimpleMove(Velocity);
        transform.eulerAngles += AngularVelocity * Time.fixedDeltaTime;
    }
    #region Motion
    public virtual Vector3 CalculateVeloctiy()
    {
        if (MotionClip != null && MotionClip.motionType == EMotionType.MT_Override)
        {
            return MotionClip.GetVelocity();
        }
        if (RootMotionClip != null && RootMotionClip.motionType == EMotionType.MT_Override)
        {
            return RootMotionClip.GetVelocity();
        }
        return MotionClip.GetVelocity() + RootMotionClip.GetVelocity() + Controller.GetInputVelocity() + Gravity;
    }

    public virtual Vector3 CalculateAngularVelocity()
    {
        if (MotionClip != null && MotionClip.rotateType == EMotionType.MT_Override)
        {
            return MotionClip.GetAngularVelocity();
        }
        if (RootMotionClip != null && RootMotionClip.rotateType == EMotionType.MT_Override)
        {
            return RootMotionClip.GetAngularVelocity();
        }
        return MotionClip.GetAngularVelocity() + RootMotionClip.GetAngularVelocity() + Controller.GetInputAngularVelocity();
    }

    public bool CanApplyMotion(int priority)
    {
        return MotionClip.priority <= priority;
    }

    public void RegisterMotionEvent(ELifeType lifeType, UnityAction OnMotionEvent)
    {
        switch (lifeType)
        {
            case ELifeType.LT_Start:
                MotionClip.OnMotionStart += OnMotionEvent;
                break;
            case ELifeType.LT_Loop:
                MotionClip.OnMotionUpdate += OnMotionEvent;
                break;
            case ELifeType.LT_End:
                MotionClip.OnMotionEnd += OnMotionEvent;
                break;
        }
    } 
    public void RemoveMotionEvent(ELifeType lifeType, UnityAction OnMotionEvent)
    {
        switch (lifeType)
        {
            case ELifeType.LT_Start:
                MotionClip.OnMotionStart -= OnMotionEvent;
                break;
            case ELifeType.LT_Loop:
                MotionClip.OnMotionUpdate -= OnMotionEvent;
                break;
            case ELifeType.LT_End:
                MotionClip.OnMotionEnd -= OnMotionEvent;
                break;
        }
    }
    public void StartMotion()
    {
        MotionClip.StartMotion();
    }
    public void ApplyMotion(int priority, EMotionType motionType, EDirectType directType, float distance, float duration = 0.0f, AnimationCurve moveCurve = null)
    {
        MotionClip.ApplyMotion(priority, motionType, directType, distance, duration, moveCurve);
    }
    public void ApplyRotate(int priority, EMotionType rotateType, EDirectType asixType, float rotateAngle, float duration = 0.0f)
    {
        MotionClip.ApplyRotate(priority, rotateType, asixType, rotateAngle, duration);
    }
    public void ApplyRotate(int priority, EMotionType rotateType, Quaternion startRot, EDirectType rotateAxis, float rotateAngle, float duration = 0.0f, AnimationCurve rotateCurve = null)
    {
        MotionClip.ApplyRotate(priority, rotateType, startRot, rotateAxis, rotateAngle, duration, rotateCurve);
    }
    #endregion
}