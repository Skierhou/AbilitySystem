using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Assets/Scripts/AbilitySystem/Datas/_AbilityConsts", menuName = "Ability/Create_ConstsConfig", order = 0)]
[System.Serializable]
public class AbilityConsts : ScriptableObject
{
    public static AbilityConsts _instance;
    public static AbilityConsts Instance { get {
            if (_instance is null)
            {
                _instance = GetInstance();
            }
            return _instance;
        } }

    public List<string> abilityPathList;

    public string Fire;
    public string Ice;
    public string Wood;
    public string Soil;
    public string Gold;

    static AbilityConsts GetInstance()
    {
        return Resources.Load<AbilityConsts>("Ability/_AbilityConsts");
    }
}
