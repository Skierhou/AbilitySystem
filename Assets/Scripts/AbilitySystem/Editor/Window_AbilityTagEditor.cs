using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System;

public class FTagNode
{
    public FTagNode parent;
    public List<FTagNode> childs;
    public bool bToggle;
    public bool bOpen = true;
    public string name;

    public FTagNode(string name,FTagNode parent)
    {
        this.name = name;
        this.parent = parent;
        childs = new List<FTagNode>();
    }

    public void OnToggleChanged(bool inToggle)
    {
        bToggle = inToggle;
        if (inToggle)
        {
            FTagNode temp = parent;
            while (temp != null)
            {
                temp.bToggle = true;
                temp = temp.parent;
            }
        }
        else
        {
            OnToggleOff();
        }
        Window_AbilityTagEditor.OnValueChanged?.Invoke(Window_AbilityTagEditor.GetSelectedTags());
    }

    void OnToggleOff()
    {
        bToggle = false;
        for (int i = 0; i < childs.Count; i++)
        {
            childs[i].OnToggleOff();
        }
    }

    public string GetTagTotalName()
    {
        List<string> tempList = new List<string>();
        FTagNode node = this;
        while (node != null)
        {
            tempList.Add(node.name);
            node = node.parent;
        }
        string res = "";
        for (int i = tempList.Count - 1; i >= 0; i--)
        {
            res += tempList[i] + (i == 0 ? "" : ".");
        }
        return res;
    }
};

public class Window_AbilityTagEditor : EditorWindow
{
    //private const string ABILITYTAGCONFIGPATH = UnityEngine.Application.dataPath + "/Configs";
    private const string CONFIGNAME = "AbilityTag.ini";

    public static Action<List<string>> OnValueChanged;

    string m_String;

    static List<FTagNode> nodeList = new List<FTagNode>();

    //[MenuItem("EditorGUILayout/Tag2")]
    public static void Init(List<string> nowTagList = null,Action<List<string>> valueChangedCallback = null)
    {
        ReadConfig();

        OnValueChanged = valueChangedCallback;
        Window_AbilityTagEditor window = (Window_AbilityTagEditor)EditorWindow.GetWindow(typeof(Window_AbilityTagEditor));
        window.Show();

        InitToggle(nowTagList);
    }

    static void InitToggle(List<string> nowTagList)
    {
        for (int i = 0; i < nowTagList.Count; i++)
        {
            string[] strs = nowTagList[i].Split('.');
            InitToggleTag(strs);
        }
    }
    static void InitToggleTag(string[] strs)
    {
        if (strs == null || strs.Length == 0) return;

        int index = 1;
        FTagNode node = null;
        for (int i = 0; i < nodeList.Count; i++)
        {
            if (nodeList[i].name.Equals(strs[0]))
            {
                node = nodeList[i];
            }
        }

        while (index < strs.Length && node != null)
        {
            for (int i = 0; i < node.childs.Count; i++)
            {
                if (node.childs[i].name.Equals(strs[index]))
                {
                    node = node.childs[i];
                    break;
                }
            }
            ++index;
        }
        node.OnToggleChanged(true);
    }

    static void ReadConfig()
    {
        nodeList.Clear();
        string pp = UnityEngine.Application.dataPath + "/Configs";
        if (!Directory.Exists(pp))
        {
            Directory.CreateDirectory(pp);
        }
        DirectoryInfo tDirectoryInfo = new DirectoryInfo(pp);

        string path = pp + "/" + CONFIGNAME;
        if (!File.Exists(path))
            File.Create(path);
        if (File.Exists(path))
        {
            using (FileStream file = File.OpenRead(path))
            {
                using (StreamReader tReader = new StreamReader(file))
                {
                    while (!tReader.EndOfStream)
                    {
                        string tStr = Regex.Replace(tReader.ReadLine(), @"\s", "");
                        AddTag(tStr);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Return selected Tags
    /// </summary>
    public static List<string> GetSelectedTags()
    {
        List<string> strList = new List<string>();
        for (int i = 0; i < nodeList.Count; i++)
        {
            FindNextSelectedTag(nodeList[i], strList);
        }
        return strList;
    }

    static void FindNextSelectedTag(FTagNode node,List<string> strList)
    {
        if (node != null && node.bToggle)
        {
            if (node.childs == null || node.childs.Count == 0)
            {
                // TODO Found
                strList.Add(node.GetTagTotalName());
            }
            else
            {
                bool bHas = false;
                for (int i = 0; i < node.childs.Count; i++)
                {
                    bHas = (!bHas) ? node.childs[i].bToggle : bHas;
                }
                if (!bHas)
                {
                    // TODO Found
                    strList.Add(node.GetTagTotalName());
                }
                else
                {
                    for (int i = 0; i < node.childs.Count; i++)
                    {
                        FindNextSelectedTag(node.childs[i], strList);
                    }
                }
            }
        }
    }

    static void WriteConfig(string inStr)
    {
        if (string.IsNullOrEmpty(inStr) || string.IsNullOrWhiteSpace(inStr)) return;

        string pp = UnityEngine.Application.dataPath + "/Configs";
        if (!Directory.Exists(pp))
        {
            Directory.CreateDirectory(pp);
        }
        DirectoryInfo tDirectoryInfo = new DirectoryInfo(pp);

        string path = pp + "/" + CONFIGNAME;
        if (!File.Exists(path))
            File.Create(path);
        if (File.Exists(path))
        {
            using (FileStream file = File.OpenWrite(path))
            {
                file.Position = file.Length;
                using (StreamWriter tWriter = new StreamWriter(file))
                {
                    tWriter.WriteLine(inStr);
                }
            }
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        m_String = EditorGUILayout.TextField(m_String);
        if (GUILayout.Button("添加内容"))
        {
            bool bSuc = AddTag(m_String);

            if (bSuc)
                WriteConfig(m_String);
            m_String = "";
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < nodeList.Count; i++)
        {
            DrawTag(nodeList[i], 0);
        }
    }

    void DrawTag(FTagNode node,int level)
    {
        EditorGUI.indentLevel = level;
        EditorGUILayout.BeginHorizontal();
        node.bOpen = EditorGUILayout.Foldout(node.bOpen, node.name);
        bool preToggle = node.bToggle;
        node.bToggle = EditorGUILayout.ToggleLeft("", node.bToggle);
        if (preToggle != node.bToggle)
            node.OnToggleChanged(node.bToggle);
        EditorGUILayout.EndHorizontal();

        if (node.bOpen)
        {
            for (int i = 0; i < node.childs.Count; i++)
            {
                DrawTag(node.childs[i], level + 1);
            }
        }
    }

    static bool AddTag(string inStr)
    {
        if (string.IsNullOrEmpty(inStr)) return false;

        string[] strs = inStr.Split('.');
        int level = 0;
        FTagNode node = null;
        for (; level < strs.Length; level++)
        {
            FTagNode temp = CheckSame(strs[level], node);
            if (temp is null)
                break;
            else
                node = temp;
        }
        if (level == strs.Length) return false;
        if (node == null)
        {
            if (strs.Length > 0)
            {
                FTagNode root = new FTagNode(strs[0],null);
                FTagNode tagNode = root;
                for (int i = 1; i < strs.Length; i++)
                {
                    FTagNode child = new FTagNode(strs[i],tagNode);
                    tagNode.childs.Add(child);
                    tagNode = child;
                }
                nodeList.Add(root);
                return true;
            }
        }
        else
        {
            if (strs.Length > level)
            {
                FTagNode root = new FTagNode(strs[level], node);
                FTagNode tagNode = root;
                for (int i = level + 1; i < strs.Length; i++)
                {
                    FTagNode child = new FTagNode(strs[i],tagNode);
                    tagNode.childs.Add(child);
                    tagNode = child;
                }
                node.childs.Add(root);
                return true;
            }
        }
        return false;
    }

    static FTagNode CheckSame(string str, FTagNode node = null)
    {
        if (node != null)
        {
            foreach (FTagNode child in node.childs)
            {
                if (child != null && child.name.Equals(str))
                    return child;
            }
        }
        else
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i].name.Equals(str))
                    return nodeList[i];
            }
        }
        return null;
    }
}
