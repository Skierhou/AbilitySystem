using System;
using UnityEngine;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;

public static class ConfigHelper
{
    /// <summary>
    /// 所有对象读取配置方法
    /// </summary>
    public static void ReadConfig(this object inObj)
    {
        if (inObj != null)
        {
            ConfigManager.Instance.CopyConfig(inObj);
        }
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Struct, AllowMultiple = false)]
public class Config : Attribute
{
    public string ConfigFileName = null;
    public Config() { }
    public Config(string inFileName) 
    {
        ConfigFileName = inFileName;
    }
}

struct FConfigData
{
    public Type Type;
    public List<FieldInfo> FieldInfoList;
    public List<PropertyInfo> PropertyInfoList;
};

public class ConfigManager : Singleton<ConfigManager>
{
    //文件名
    private const string CONFIGPATH = "Configs";
    //注释分隔符
    private const char NOTESIGN = ';';
    //类名修饰
    private const string CLASSSIGNBEGIN = "[";
    private const string CLASSSIGNEND = "]";
    //等于号
    private const char EQUALSIGN = '=';
    //默认配置文件格式
    private const string FORMATSIGN = ".ini";
    //构造体结构
    private const char STRUCTSIGNBEGIN = '(';
    private const char STRUCTSIGNEND = ')';
    private const char STRUCTSPLITSIGN = '|';
    //列表配置
    private const char LISTBEGIN = '{';
    private const char LISTEND = '}';
    private const char LISTSPLITSIGN = ',';
    //配置文件路径
    private static string DirectionPath = UnityEngine.Application.dataPath + "/../" + CONFIGPATH;
    //存取所有类的配置信息Map
    private Dictionary<Type, Dictionary<string, string>> m_ConfigDict = new Dictionary<Type, Dictionary<string, string>>();
    //当前类型存在的配置信息列表
    private Dictionary<Type, List<FConfigData>> m_ConfigDataMap = new Dictionary<Type, List<FConfigData>>();
    //当前数据具体存在位置
    private Dictionary<Type, object> m_ConfigMap = new Dictionary<Type, object>();

    private GameObject DataGo;

    /// <summary>
    /// 初始化
    /// </summary>
    public override void Initialize()
    {
        if (!Directory.Exists(DirectionPath))
        {
            Debug.LogWarning("不存在该配置路径文件，Path : " + DirectionPath);
            return;
        }

        DirectoryInfo tDirectoryInfo = new DirectoryInfo(DirectionPath);

        if (tDirectoryInfo != null)
        {
            FileInfo[] files = tDirectoryInfo.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].FullName.EndsWith(FORMATSIGN))
                {
                    ReadFileConfig(files[i]);
                }
            }
        }
        DataGo = new GameObject("ConfigData");
        DataGo.SetActive(false);
        foreach (Type type in m_ConfigDict.Keys)
        {
            object obj;
            if (IsUnityComponent(type))
                obj = DataGo.AddComponent(type);
            else
                obj = Activator.CreateInstance(type);
            ObjectInitialize(obj);
            m_ConfigMap.Add(type, obj);
        }
    }
    /// <summary>
    /// 具体外部拷贝数据使用
    /// </summary>
    public void CopyConfig(object inObj)
    {
        Type type = inObj.GetType();
        while (IsVaildType(type))
        {
            if (m_ConfigMap.TryGetValue(type, out object obj))
            {
                List<FConfigData> configList = GetObjectConfigList(inObj);
                for (int i = configList.Count - 1; i >= 0; i--)
                {
                    foreach (FieldInfo fieldInfo in configList[i].FieldInfoList)
                    {
                        Config tConfig = fieldInfo.GetCustomAttribute<Config>();
                        if (tConfig != null)
                        {
                            if (fieldInfo.FieldType.ToString().Contains("System.Collections.Generic.List"))
                            {
                                object objList = fieldInfo.GetValue(obj);
                                object selfList = Activator.CreateInstance(fieldInfo.FieldType);
                                fieldInfo.SetValue(inObj, selfList);

                                MethodInfo methodInfo = fieldInfo.FieldType.GetMethod("AddRange", BindingFlags.Instance | BindingFlags.Public);
                                if (methodInfo != null)
                                    methodInfo.Invoke(selfList, new object[] { objList });
                            }
                            else
                            {
                                fieldInfo.SetValue(inObj, fieldInfo.GetValue(obj));
                            }
                        }
                    }
                }
                break;
            }
            else
            {
                type = type.BaseType;
            }
        }
    }

    /// <summary>
    /// 读取文件中的所有配置
    /// </summary>
    private void ReadFileConfig(FileInfo file)
    {
        if (string.IsNullOrEmpty(file.FullName))
            return;

        if (File.Exists(file.FullName))
        {
            FileStream tFS = File.Open(file.FullName, FileMode.Open);
            StreamReader tReader = new StreamReader(tFS);
            Dictionary<string, string> tConfigDict = null;
            while (!tReader.EndOfStream)
            {
                string tStr = Regex.Replace(tReader.ReadLine(), @"\s", "");

                if (tStr.StartsWith(NOTESIGN.ToString()))
                    continue;

                int signIndex = tStr.IndexOf(NOTESIGN);
                if (signIndex >= 0)
                {
                    tStr = tStr.Substring(0, signIndex);
                }
                //当前行为类名
                if (tStr.StartsWith(CLASSSIGNBEGIN) && tStr.EndsWith(CLASSSIGNEND))
                {
                    Type tType = Type.GetType(tStr.Substring(1, tStr.Length - 2), false, true);

                    if (tType != null)
                    {
                        Config config = (Config)tType.GetCustomAttribute(typeof(Config));
                        if (config != null && (config.ConfigFileName + FORMATSIGN).Equals(file.Name))
                        {
                            tConfigDict = new Dictionary<string, string>();
                            m_ConfigDict.Add(tType, tConfigDict);
                        }
                        else 
                        {
                            tConfigDict = null;
                        }
                    }
                }
                else
                {
                    if (tConfigDict != null)
                    {
                        if (tStr.Contains(STRUCTSIGNBEGIN.ToString()) && tStr.Contains(STRUCTSIGNEND.ToString()))
                        {
                            int tFirstEqualSignIndex = tStr.IndexOf(EQUALSIGN);
                            string tKey = tStr.Substring(0, tFirstEqualSignIndex);
                            string tValue = tStr.Substring(tFirstEqualSignIndex + 1, tStr.Length - tFirstEqualSignIndex - 1);

                            if (!string.IsNullOrEmpty(tValue))
                            {
                                if (tConfigDict.ContainsKey(tKey.ToUpper()))
                                {
                                    tConfigDict[tKey] += LISTSPLITSIGN + tValue;
                                }
                                else
                                {
                                    tConfigDict.Add(tKey.ToUpper(), tValue);
                                }
                            }
                        }
                        else
                        {
                            string[] tStrArray = tStr.Split(EQUALSIGN);
                            if (tStrArray.Length == 2)
                            {
                                //Key:0, Value:1
                                if (tConfigDict.ContainsKey(tStrArray[0].ToUpper()))
                                {
                                    tConfigDict[tStrArray[0].ToUpper()] += LISTSPLITSIGN + tStrArray[1];
                                }
                                else
                                {
                                    tConfigDict.Add(tStrArray[0].ToUpper(), tStrArray[1]);
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("不存在该文件，Path:" + file.FullName);
        }
    }

    /// <summary>
    /// 是否为unity组件
    /// </summary>
    /// <param name="inType"></param>
    /// <returns></returns>
    bool IsUnityComponent(Type inType)
    {
        while (inType != null)
        {
            if (inType == typeof(MonoBehaviour))
                return true;
            else if (inType == typeof(System.Object) || inType == typeof(UnityEngine.Object))
                return false;
            inType = inType.BaseType;
        }
        return false;
    }

    /// <summary>
    /// 是否为有效类型
    /// </summary>
    bool IsVaildType(Type inType)
    {
        return !(inType == typeof(MonoBehaviour) || inType == typeof(System.Object) || inType == typeof(UnityEngine.Object));
    }

    /// <summary>
    /// 获取当前类型需要配置的数据列表
    /// </summary>
    List<FConfigData> GetObjectConfigList(object inObj)
    {
        if (m_ConfigDataMap.TryGetValue(inObj.GetType(), out List<FConfigData> res) && res != null)
            return res;

        List<FConfigData> list = new List<FConfigData>();
        List<FieldInfo> tFieldInfoList;
        List<PropertyInfo> tPropertyInfoList;

        Type tType = inObj.GetType();

        while (tType != null && tType != typeof(MonoBehaviour) && tType != typeof(System.Object) && tType != typeof(UnityEngine.Object))
        {
            Config config = (Config)tType.GetCustomAttribute(typeof(Config));
            if (config != null && !string.IsNullOrEmpty(config.ConfigFileName))
            {
                FieldInfo[] tFieldInfos = tType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly
                | BindingFlags.NonPublic | BindingFlags.Public);
                PropertyInfo[] tProperties = tType.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly
                    | BindingFlags.NonPublic | BindingFlags.Public);

                tFieldInfoList = new List<FieldInfo>();
                tPropertyInfoList = new List<PropertyInfo>();
                if (tFieldInfos.Length > 0)
                {
                    tFieldInfoList.AddRange(tFieldInfos);
                }
                if (tProperties.Length > 0)
                {
                    tPropertyInfoList.AddRange(tProperties);
                }
                list.Add(new FConfigData { Type = tType, FieldInfoList = tFieldInfoList, PropertyInfoList = tPropertyInfoList });
            }
            tType = tType.BaseType;
        }

        tFieldInfoList = new List<FieldInfo>();
        tPropertyInfoList = new List<PropertyInfo>();
        for (int i = list.Count - 1; i >= 0; i--)
        {
            FConfigData configData = list[i];
            tFieldInfoList.AddRange(configData.FieldInfoList);
            tPropertyInfoList.AddRange(configData.PropertyInfoList);
            configData.FieldInfoList = tFieldInfoList;
            configData.PropertyInfoList = tPropertyInfoList;
            list[i] = configData;
        }
        m_ConfigDataMap.Add(inObj.GetType(), list);
        return list;
    }

    /// <summary>
    /// 类初始化调用
    /// </summary>
    void ObjectInitialize(object inObj)
    {
        List<FConfigData> list = GetObjectConfigList(inObj);

        for (int i = list.Count - 1; i >= 0; i--)
        {
            foreach (FieldInfo fieldInfo in list[i].FieldInfoList)
            {
                Config tConfig = fieldInfo.GetCustomAttribute<Config>();
                if (tConfig != null)
                {
                    SetObjectProperty(inObj, fieldInfo, list[i].Type);
                }
            }
        }
    }

    /// <summary>
    /// 获取List列表存放的数据类型
    /// </summary>
    private Type GetListType(string inTypeStr)
    {
        int startIndex = inTypeStr.LastIndexOf('[') + 1;
        string type = inTypeStr.Substring(startIndex, inTypeStr.LastIndexOf(']') - startIndex);
        return Type.GetType(type, true, true);
    }

    /// <summary>
    /// 设置构造体内字段值
    /// </summary>
    /// <param name="inObj">一个类里的构造体对象</param>
    /// <param name="inValue">值:格式(X=1,Y=2,Z=3)</param>
    private void SetStructProperty(ref object inObj, string inValue)
    {
        string[] tValueArray = inValue.Split(STRUCTSPLITSIGN);

        FieldInfo[] tFieldInfos = inObj.GetType().GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly
            | BindingFlags.NonPublic | BindingFlags.Public);

        for (int i = 0; i < tValueArray.Length; i++)
        {
            string[] tStrArray = tValueArray[i].Split(EQUALSIGN);

            if (tStrArray.Length == 2)
            {
                for (int tFieldIndex = 0; tFieldIndex < tFieldInfos.Length; tFieldIndex++)
                {
                    if (tFieldInfos[tFieldIndex].Name.ToUpper().Equals(tStrArray[0].ToUpper()))
                    {
                        tFieldInfos[tFieldIndex].SetValue(inObj, Convert.ChangeType(tStrArray[1], tFieldInfos[tFieldIndex].FieldType));
                    }
                }
            }
        }
    }

    /// <summary>
    /// 设置具体属性
    /// </summary>
    private void SetObjectProperty(object inObj, FieldInfo inInfo, Type inType)
    {
        if (m_ConfigDict.TryGetValue(inType, out Dictionary<string, string> tDict) && tDict != null)
        {
            if (tDict.TryGetValue(inInfo.Name.ToUpper(),out string tStr) && !string.IsNullOrEmpty(tStr))
            {
                try
                {
                    if (CheckIsList(tStr) && inInfo.FieldType.ToString().Contains("System.Collections.Generic.List"))
                    {
                        string[] tStrArray = tStr.Substring(1, tStr.Length - 2).Split(LISTSPLITSIGN);
                        Type type = GetListType(inInfo.FieldType.ToString());
                        object entityList = Activator.CreateInstance(inInfo.FieldType);
                        MethodInfo methodInfo = inInfo.FieldType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);

                        for (int i = 0; i < tStrArray.Length; i++)
                        {
                            if (CheckIsStruct(tStrArray[i]))
                            {
                                object structObj = Activator.CreateInstance(type);
                                SetStructProperty(ref structObj, tStrArray[i].Substring(1, tStrArray[i].Length - 2));
                                methodInfo.Invoke(entityList, new object[] { Convert.ChangeType(structObj, type) });
                            }
                            else
                            {
                                methodInfo.Invoke(entityList, new object[] { Convert.ChangeType(tStrArray[i], type) });//相当于List<T>调用Add方法
                            }
                        }
                        inInfo.SetValue(inObj, entityList);
                    }
                    else
                    {
                        if (CheckIsStruct(tStr))
                        {
                            object structObj = inInfo.GetValue(inObj);
                            SetStructProperty(ref structObj, tStr.Substring(1, tStr.Length - 2));
                            inInfo.SetValue(inObj, structObj);
                        }
                        else
                        {
                            inInfo.SetValue(inObj, Convert.ChangeType(tStr, inInfo.FieldType));
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning(string.Format("无法将{0}转成{1}类型! 报错:{2}", tStr, inInfo.FieldType.ToString(), e.Message));
                }
            }
        }
    }

    private bool CheckIsStruct(string inStr)
    {
        return !string.IsNullOrEmpty(inStr) && inStr[0] == STRUCTSIGNBEGIN && inStr[inStr.Length - 1] == STRUCTSIGNEND;
    }
    private bool CheckIsList(string inStr)
    {
        return !string.IsNullOrEmpty(inStr) && inStr[0] == LISTBEGIN && inStr[inStr.Length - 1] == LISTEND;
    }
}
