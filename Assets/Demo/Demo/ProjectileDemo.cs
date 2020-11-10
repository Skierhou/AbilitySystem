using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDemo : MonoBehaviour
{
    public Transform target;
    public GameObject projectileTemplate;
    public GameObject bounceProjectileTemplate;

    public AnimationCurve curve;

    void Start()
    {
        StartCoroutine(Fire());
    }

    IEnumerator Fire()
    {
        int index = 0;
        while (true)
        {
            Projectile projectile;
            if (index == 4)
                projectile = Instantiate(bounceProjectileTemplate, transform.position, transform.rotation).GetComponent<Projectile>();
            else
                projectile = Instantiate(projectileTemplate, transform.position, transform.rotation).GetComponent<Projectile>();
            switch (index)
            {
                case 0:
                    projectile.InitProjByDirection(transform.position, target.position - transform.position, 20, 30);
                    projectile.InitLookAt(true);
                    break;
                case 1:
                    projectile.GravityScale = 3.0f;
                    projectile.InitProjByFixedPoint(transform.position, target.position, 10, 15);
                    projectile.InitLookAt(true);
                    break;
                case 2:
                    projectile.InitProjByTarget(transform.position, target, 15, 20, 5, 90, 180);
                    break;
                case 3:
                    FProjectileCurveData data1 = new FProjectileCurveData(EDirectType.DT_RelativeForward, curve, 1.0f, 2.0f);
                    projectile.InitProjByCurve(transform.position, target.position, data1);
                    projectile.InitLookAt(true);
                    break;
                case 4:
                    projectile.GravityScale = 2.0f;
                    projectile.LifeSpan = 5.0f;
                    projectile.InitProjByFixedPoint(transform.position, target.position, 10, 15);
                    projectile.InitLookAt(true);
                    break;
            }
            projectile.Launch();
            ++index; index %= 5;
            yield return new WaitForSeconds(2);
        }
    }
}
