using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

/// <summary>
/// Unity 5.6
/// </summary>
public class EditorGUILayoutExample : EditorWindow
{
    #region BeginFadeGroup
    AnimBool m_ShowExtraFields;
    string m_String;
    Color m_Color = Color.white;
    int m_Number = 0;
    #endregion

    #region BeginScrollView
    Vector2 scrollPos;
    string m_content = "";
    string t = "这 是 一 个 测 试 Scroll view 的 文 本 ！\n";
    #endregion

    #region BeginToggleGroup
    bool[] pos = new bool[3] { true, true, true };

    bool posGroupEnabled = true;
    #endregion

    #region Foldout
    bool showFoldout = true;
    #endregion

    [MenuItem("EditorGUILayout/EditorGUILayoutExample")]
    static void Init()
    {
        EditorGUILayoutExample window = (EditorGUILayoutExample)EditorWindow.GetWindow(typeof(EditorGUILayoutExample));
        window.Show();
    }

    void OnEnable()
    {
        #region  BeginFadeGroup
        m_ShowExtraFields = new AnimBool(true);//创建一个AnimBool对象，true是默认显示。
        m_ShowExtraFields.valueChanged.AddListener(Repaint);//监听重绘
        #endregion
    }
    void OnGUI()
    {
        #region BeginFadeGroup
        m_ShowExtraFields.target = EditorGUILayout.ToggleLeft("显示折叠内容", m_ShowExtraFields.target);//选择框在左边的开关
        m_ShowExtraFields.target = EditorGUILayout.Toggle("显示折叠内容", m_ShowExtraFields.target);//选择框在右边的开关
        //创建带渐显动画的折叠块 返回值bool，参数float
        if (EditorGUILayout.BeginFadeGroup(m_ShowExtraFields.faded))
        {
            EditorGUI.indentLevel++;//缩进深度增加，以下的GUI会增加缩进

            EditorGUILayout.LabelField("ColorColorColorColorColorColorColorColorColorColor");//标签栏

            EditorGUILayout.PrefixLabel("ColorColorColorColorColorColorColorColorColorColor");//前缀标签
            m_Color = EditorGUILayout.ColorField(m_Color);

            EditorGUILayout.PrefixLabel("Text");
            m_String = EditorGUILayout.TextField(m_String);//文本框

            EditorGUILayout.PrefixLabel("Number");
            m_Number = EditorGUILayout.IntSlider(m_Number, 0, 10);//Int滑动条

            EditorGUI.indentLevel--;//缩进深度减少，以下的GUI会减少缩进
        }
        EditorGUILayout.EndFadeGroup();
        #endregion
        #region Foldout
        showFoldout = EditorGUILayout.Foldout(showFoldout, "折叠子物体：");
        if (showFoldout)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("折叠块内容1");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("折叠块内容2");
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("折叠块内容3");
        }
        #endregion

        #region BeginHorizontal 水平布局
        Rect r = EditorGUILayout.BeginHorizontal("Button");
        if (GUI.Button(r, GUIContent.none))
            Debug.Log("Go here");
        GUILayout.Label("I'm inside the button");
        GUILayout.Label("So am I");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("第一个内容");
        GUILayout.Label("第二个内容");
        if (GUILayout.Button("第三个按钮"))
        {
            Debug.Log("GUILayout的按钮");
        }
        EditorGUILayout.EndHorizontal();
        #endregion

        #region BeginVertical 垂直布局
        EditorGUILayout.BeginVertical();
        GUILayout.Label("第一个内容");
        GUILayout.Label("第二个内容");
        if (GUILayout.Button("第三个按钮"))
        {
            Debug.Log("GUILayout的按钮");
        }
        EditorGUILayout.EndVertical();
        #endregion

        #region BeginScrollView
        //需要将返回值赋值到临时变量，不然拖不动
        //可以添加GUILayoutOption参数控制大小
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(200), GUILayout.Height(100));
        GUILayout.Label(m_content);
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加内容"))
            m_content += t;
        if (GUILayout.Button("清空内容"))
            m_content = "";
        EditorGUILayout.EndHorizontal();
        #endregion

        #region BeginToggleGroup

        posGroupEnabled = EditorGUILayout.BeginToggleGroup("ToggleGroup", posGroupEnabled);
        EditorGUILayout.BeginVertical();
        pos[0] = EditorGUILayout.Toggle("Toggle1", pos[0]);
        pos[1] = EditorGUILayout.Toggle("Toggle2", pos[1]);
        pos[2] = EditorGUILayout.Toggle("Toggle3", pos[2]);
        if (GUILayout.Button("添加内容"))
            m_content += t;
        m_String = EditorGUILayout.TextField(m_String);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndToggleGroup();

        #endregion

        //画一个居中的分割线
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("-----------------分割线-----------------");
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

    }

}