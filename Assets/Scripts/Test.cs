using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Rigidbody m_Rigidbody;
    public Projectile projectile;

    public Transform target;

    public AnimationCurve fwd_curve;
    public AnimationCurve up_curve;
    // Start is called before the first frame update
    void Start()
    {
        //FProjectileCurveData data1 = new FProjectileCurveData(EProjectileCurve.PC_RelativeForward, fwd_curve, 1.0f, 2.0f);
        //FProjectileCurveData data2 = new FProjectileCurveData(EProjectileCurve.PC_WorldUp, up_curve, 50.0f, 2.0f);
        //projectile.InitProjByCurve(transform.position, target.position, data1,data2);

        //projectile.InitProjByDirection(transform.position, target.position - transform.position, 20, 30);
        //projectile.UseGravity = true;
        //projectile.InitProjByFixedPoint(transform.position, target.position, 10, 15);
        projectile.InitProjByTarget(transform.position, target, 0, 20, 5, 10, 20);
        //projectile.InitLookAt(true);
        projectile.Launch();
    }

    // Update is called once per frame
    void Update()
    {
        //m_Rigidbody.angularVelocity = new Vector3(0, 30, 0) * Time.deltaTime;
    }
}
