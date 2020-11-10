using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public interface IProjectile
{
    /// <summary>
    /// 直线飞行
    /// </summary>
    void InitProjByDirection(Vector3 inStartLoc, Vector3 inDir, float inSpeed, float inMaxSpeed, float inAccelerate);
    /// <summary>
    /// 固定点曲线/直线飞行
    /// </summary>
    void InitProjByFixedPoint(Vector3 inStartLoc, Vector3 inEndLoc, float inSpeed, float inAccelerate);
    /// <summary>
    /// 跟踪目标飞行
    /// </summary>
    void InitProjByTarget(Vector3 inStartLoc, Transform inTarget, float inSpeed, float inMaxSpeed, float inAccel, float inAngularSpeed, float inMaxAngularSpeed);
    /// <summary>
    /// 沿动画曲线飞行到目标点
    /// </summary>
    void InitProjByCurve(Vector3 inStartLoc, Vector3 inTargetLoc, params FProjectileCurveData[] curveDatas);
    /// <summary>
    /// 初始化飞行过程朝向
    /// </summary>
    void InitLookAt(Transform inTarget);
    /// <summary>
    /// 初始化飞行过程朝向
    /// </summary>
    void InitLookAt(Vector3 inTargetLoc);
    /// <summary>
    /// 初始化飞行过程朝向
    /// </summary>
    void InitLookAt(bool bUseDirectVelocity);

    /// <summary>
    /// 发射
    /// </summary>
    void Launch();
    /// <summary>
    /// 着落
    /// </summary>
    void Landed(Vector3 inLoc);
};

public enum EProjectileType
{
    PT_Normal,
    PT_Target,
    PT_Curvy,
}

public enum EDirectType
{
    DT_RelativeForward,
    DT_RelativeRight,
    DT_RelativeUp,
    DT_WorldForward,
    DT_WorldRight,
    DT_WorldUp,
    DT_SelfForward,
    DT_SelfRight,
    DT_SelfUp,
}

public struct FProjectileCurveData
{
    public EDirectType curveType;
    public AnimationCurve curve;
    public float curveTimer;
    public float curveSmoothing;
    public float curveScale;

    public FProjectileCurveData(EDirectType curveType, AnimationCurve curve, float curveScale, float flyTotalTime)
    {
        this.curve = curve;
        this.curveType = curveType;
        this.curveScale = curveScale;
        this.curveSmoothing = curve.keys[curve.length - 1].time / flyTotalTime;
        this.curveTimer = 0;
    }
}

public struct FProjectileData
{
    public EProjectileType projectileType;

    /* 影响实际位移 */
    public Vector3 velocity;
    public Vector3 gravity;

    public float gravityScale;

    /* 速度 */
    public float accelerate;
    public float speed;
    public float maxSpeed;

    /* 角速度 */
    public float angularSpeed;
    public float maxAngularSpeed;

    /* 曲线 */
    public List<FProjectileCurveData> curveDatas;

    /* 三个锁定朝向，优先级Lookat>TargetLoc>IsAutoLook */
    public Transform Target;
    public Vector3 StartLoc;
    public Vector3 TargetLoc;
    /* 是否锁定目标地点 */
    public bool IsLookTargetLoc;
    /* 是否自动锁定(沿飞行方向) */
    public bool IsDirectVelocity;

    /* 起始点->目标点的相对朝向 */
    public Vector3 relativeForward;
    public Vector3 relativeRight;
    public Vector3 relativeUp;
};

public class Projectile : MonoBehaviour,IProjectile
{
    /* Base */
    public float LifeSpan { set; get; } = 1000;
    public float GravityScale { get => projectileData.gravityScale; set => projectileData.gravityScale = value; }
    public float CurrGravityScale { protected set; get; }
    public bool IsFly { get; protected set; }

    public UnityAction OnProjectile_Landed;

    /* Data */
    protected FProjectileData projectileData;

    /* Component */
    protected Rigidbody m_Rigidbody;

    #region Interface
    public virtual void InitProjByDirection(Vector3 inStartLoc, Vector3 inDir, float inSpeed
        , float inMaxSpeed, float inAccelerate = 0.0f)
    {
        projectileData.projectileType = EProjectileType.PT_Normal;
        transform.forward = inDir.normalized;
        projectileData.relativeForward = transform.forward;

        projectileData.StartLoc = inStartLoc;
        projectileData.accelerate = inAccelerate;
        projectileData.velocity = inDir.normalized * inSpeed;
        projectileData.speed = inSpeed;
        projectileData.maxSpeed = inMaxSpeed;
    }
    
    public virtual void InitProjByFixedPoint(Vector3 inStartLoc, Vector3 inEndLoc, float inSpeed, float inAccelerate)
    {
        Vector3 VX, VY, TempDist;
        float DeltaX, DeltaY, Vox, Voy, G, T;

        // self
        projectileData.projectileType = EProjectileType.PT_Normal;
        transform.position = inStartLoc;
        projectileData.StartLoc = inStartLoc;
        projectileData.accelerate = inAccelerate;
        projectileData.TargetLoc = inEndLoc;
        projectileData.maxSpeed = float.MaxValue;
        projectileData.speed = inSpeed;

        // calculate
        G = -GetGravity();
        Vox = inSpeed;
        TempDist = inEndLoc - inStartLoc;
        TempDist.y = 0;
        DeltaX = Vector3.Distance(inStartLoc, inEndLoc);
        DeltaY = inEndLoc.y - inStartLoc.y;
        T = (-Vox + Mathf.Sqrt(Vox * Vox + 2 * projectileData.accelerate * DeltaX)) / projectileData.accelerate;

        Voy = (DeltaY - 0.5f * G * T * T) / T;
        VY = Vector3.up * Voy;

        VX = TempDist.normalized * Vox;
        projectileData.velocity = VX + VY;

        projectileData.gravity = VY;
        projectileData.relativeForward = TempDist.normalized;
    }
   
    public virtual void InitProjByTarget(Vector3 inStartLoc, Transform inTarget, float inSpeed, float inMaxSpeed, float inAccel
        , float inAngularSpeed, float inMaxAngularSpeed)
    {
        projectileData.projectileType = EProjectileType.PT_Target;
        projectileData.velocity = transform.forward;
        transform.position = inStartLoc;
        projectileData.StartLoc = inStartLoc;
        projectileData.Target = inTarget;
        projectileData.speed = inSpeed;
        projectileData.maxSpeed = inMaxSpeed;
        projectileData.accelerate = inAccel;
        projectileData.angularSpeed = inAngularSpeed;
        projectileData.maxAngularSpeed = inMaxAngularSpeed;
    }
    
    public virtual void InitProjByCurve(Vector3 inStartLoc, Vector3 inTargetLoc, params FProjectileCurveData[] curveDatas)
    {
        projectileData.projectileType = EProjectileType.PT_Curvy;
        transform.rotation = Quaternion.LookRotation(inTargetLoc - inStartLoc, Vector3.up);
        projectileData.relativeForward = transform.forward;
        projectileData.relativeRight = transform.right;
        projectileData.relativeUp = transform.up;
        projectileData.StartLoc = inStartLoc;
        projectileData.TargetLoc = inTargetLoc;
        projectileData.curveDatas = curveDatas.ToList();
    }
    
    public virtual void InitLookAt(Transform inTarget)
    {
        projectileData.Target = inTarget;
    }
    public virtual void InitLookAt(Vector3 inTargetLoc)
    {
        projectileData.IsLookTargetLoc = true;
        projectileData.TargetLoc = inTargetLoc;
    }
    public virtual void InitLookAt(bool bUseDirectVelocity)
    {
        projectileData.IsDirectVelocity = bUseDirectVelocity;
    }

    public virtual void Launch()
    {
        IsFly = true;
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.velocity = projectileData.velocity;
    }
    public virtual void Landed(Vector3 inLoc)
    {
        IsFly = false;
        m_Rigidbody.isKinematic = true;
        m_Rigidbody.velocity = Vector3.zero;

        OnProjectile_Landed?.Invoke();

        Destroy(this.gameObject);
    }
    #endregion

    #region Fly
    protected virtual float GetGravity()
    {
        return -Physics.gravity.y * GravityScale;
    }

    /// <summary>
    /// 普通直线/曲线模式
    /// </summary>
    protected virtual void Fly_Normal(float inDeltaTime)
    {
        projectileData.speed += projectileData.accelerate * inDeltaTime;
        projectileData.speed = Mathf.Clamp(projectileData.speed, 0, projectileData.maxSpeed);
        projectileData.gravity += GetGravity() * Vector3.down * inDeltaTime;
        projectileData.velocity = projectileData.relativeForward * projectileData.speed + projectileData.gravity;

        m_Rigidbody.velocity = projectileData.velocity;
    }
    /// <summary>
    /// 跟踪目标模式
    /// </summary>
    protected virtual void Fly_Target(float inDeltaTime)
    {
        projectileData.velocity = m_Rigidbody.velocity;

        // speed
        projectileData.speed += projectileData.accelerate * inDeltaTime;
        projectileData.speed = Mathf.Clamp(projectileData.speed, 0, projectileData.maxSpeed);

        // angular
        float angularSpeed = Mathf.Lerp(projectileData.angularSpeed,projectileData.maxAngularSpeed, projectileData.speed / projectileData.maxSpeed);

        m_Rigidbody.velocity = projectileData.speed * transform.forward.normalized;
        float rate = angularSpeed / (MathEx.ClampVector360(Quaternion.LookRotation(projectileData.Target.position - transform.position).eulerAngles) - MathEx.ClampVector360(transform.rotation.eulerAngles)).magnitude;
        Quaternion rot = Quaternion.LookRotation(projectileData.Target.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, rate * inDeltaTime);
    }
    /// <summary>
    /// 按曲线差值
    /// </summary>
    protected virtual void Fly_Curve(float inDeltaTime)
    {
        if (projectileData.curveDatas == null) return;
        Vector3 delta = Vector3.zero;
        Vector3 delta2 = Vector3.zero;      //delta2 为了计算当前速度方向
        for (int i = 0; i < projectileData.curveDatas.Count; i++)
        {
            FProjectileCurveData data = projectileData.curveDatas[i];
            data.curveTimer += inDeltaTime * data.curveSmoothing;
            data.curveTimer = Mathf.Clamp(data.curveTimer, 0, data.curve.keys[data.curve.length - 1].time);
            projectileData.curveDatas[i] = data;
            delta += GetCurveDelta(data);

            data.curveTimer -= inDeltaTime * data.curveSmoothing;
            data.curveTimer = Mathf.Clamp(data.curveTimer, 0, data.curve.keys[data.curve.length - 1].time);
            delta2 += GetCurveDelta(data);
        }
        projectileData.velocity = delta - delta2;
        transform.position = projectileData.StartLoc + delta;
    }
    protected Vector3 GetCurveDelta(FProjectileCurveData data)
    {
        Vector3 delta = Vector3.zero;
        float precent = data.curve.Evaluate(data.curveTimer);
        switch (data.curveType)
        {
            case EDirectType.DT_RelativeForward:
                delta += (projectileData.TargetLoc - projectileData.StartLoc) * precent;
                break;
            case EDirectType.DT_RelativeRight:
                delta += projectileData.relativeRight * precent * data.curveScale;
                break;
            case EDirectType.DT_RelativeUp:
                delta += projectileData.relativeUp * precent * data.curveScale;
                break;
            case EDirectType.DT_WorldForward:
                delta += Vector3.forward * precent * data.curveScale;
                break;
            case EDirectType.DT_WorldRight:
                delta += Vector3.right * precent * data.curveScale;
                break;
            case EDirectType.DT_WorldUp:
                delta += Vector3.up * precent * data.curveScale;
                break;
        }
        return delta;
    }
    #endregion

    #region Unity Life
    protected virtual void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        if (m_Rigidbody != null)
        {
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
        }
    }
    protected virtual void Update()
    {
        if (IsFly)
        {
            switch (projectileData.projectileType)
            {
                case EProjectileType.PT_Normal:
                    Fly_Normal(Time.deltaTime);
                    break;
                case EProjectileType.PT_Target:
                    Fly_Target(Time.deltaTime);
                    break;
                case EProjectileType.PT_Curvy:
                    Fly_Curve(Time.deltaTime);
                    break;
            }
        }
        if (LifeSpan > 0)
            LifeSpan -= Time.deltaTime;
        else
            Landed(transform.position);
    }
    protected virtual void LateUpdate()
    {
        if (projectileData.projectileType != EProjectileType.PT_Target)
        {
            if (projectileData.Target != null)
            {
                transform.LookAt(projectileData.Target);
            }
            else
            {
                if (projectileData.IsLookTargetLoc)
                {
                    transform.LookAt(projectileData.TargetLoc);
                }
                else
                {
                    if (projectileData.IsDirectVelocity)
                    {
                        transform.LookAt(transform.position + projectileData.velocity);
                    }
                }
            }
        }
        Debug.DrawLine(transform.position, transform.position + projectileData.velocity * projectileData.speed, Color.red, Time.deltaTime);
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (IsFly)
        {
            Landed(transform.position);
        }
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (IsFly)
        {
            Landed(transform.position);
        }
    }
    #endregion
}
