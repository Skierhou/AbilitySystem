using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Resources/_AnimationCurveManager", menuName = "Ability/Create_AnimationCurveManager", order = 0)]
[System.Serializable]
public class AnimationCurveManager : ScriptableObject
{
    public static AnimationCurveManager _instance;
    public static AnimationCurveManager Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = GetInstance();
            }
            return _instance;
        }
    }
    static AnimationCurveManager GetInstance()
    {
        return Resources.Load<AnimationCurveManager>("_AnimationCurveManager");
    }
    public List<AnimationCurve> Curves;

    public AnimationCurve GetCurve(int index)
    {
        if (index < 0 || index >= Curves.Count) return null;
        return Curves[index];
    }
}