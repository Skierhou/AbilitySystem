using System;
using System.Collections.Generic;

public enum ETaskType
{
    TT_Main,
    TT_Branch,
    TT_Achievement,
};

public enum ETaskStatus
{
    TS_UnAccepted,
    TS_Accepted,
    TS_Finished,
}

public class Task
{
    public int Id;
    public string Des;
    public ETaskType TaskType;
    public ETaskStatus TaskStatus;

    /* 任务开启后监听的事件 */
    public List<FAbilityTagContainer> registerEventTags;
    public float Current;
    public float Conditions;

    /* 需要触发的任务标签 */
    public List<FAbilityTagContainer> needTriggerTaskTags;
    /* 已经触发的任务标签 */
    public List<FAbilityTagContainer> alreadyTriggerTaskTags;
    /* 完成后触发的任务标签 */
    public List<FAbilityTagContainer> finishTriggerTaskTags;

    /// <summary>
    /// 尝试触发任务
    /// </summary>
    public bool TryTrigger(FAbilityTagContainer inTag)
    {
        if (TaskStatus != ETaskStatus.TS_UnAccepted) return false;

        bool bRes = false;

        if (needTriggerTaskTags == null)
        {
            bRes = true;
            TaskStatus = ETaskStatus.TS_Accepted;
        }
        else if (needTriggerTaskTags.Contains(inTag))
        {
            if (alreadyTriggerTaskTags == null)
                alreadyTriggerTaskTags = new List<FAbilityTagContainer>();

            alreadyTriggerTaskTags.Add(inTag);

            if (alreadyTriggerTaskTags.Count == needTriggerTaskTags.Count)
            {
                foreach (FAbilityTagContainer registerTag in registerEventTags)
                {
                    AbilityManager.Instance.RegisterEvent(registerTag, OnTriggerEvent);
                }
                bRes = true;
                TaskStatus = ETaskStatus.TS_Accepted;
            }
        }
        return bRes;
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    /// <param name="param3"></param>
    public void OnTriggerEvent(FAbilityTagContainer inTriggerTag,object param1, object param2, object param3)
    {
        float add = (float)param1;

        Current += add;
        if (Current >= Conditions)
        {
            TaskStatus = ETaskStatus.TS_Finished;
            TaskManager.Instance.RemoveTask(this);

            foreach (FAbilityTagContainer registerTag in registerEventTags)
            {
                AbilityManager.Instance.RemoveEvent(registerTag, OnTriggerEvent);
            }
            foreach (FAbilityTagContainer triggerTag in finishTriggerTaskTags)
            {
                AbilityManager.Instance.TriggerEvent(triggerTag, 1);
            }
        }
    }
}

public class TaskManager:Singleton<TaskManager>
{
    /* 所有任务 */
    public Dictionary<int, Task> m_TaskMap;
    /* 当前已获取的任务 */
    public Dictionary<int, Task> m_CurrentTaskMap;
    /* 未接受的任务 */
    public Dictionary<FAbilityTagContainer, List<Task>> m_UnAcceptTaskMap;

    public override void Initialize()
    {
        AbilityManager.Instance.onTriggerEvent += OnTriggerEvent;
    }

    public void OnTriggerEvent(FAbilityTagContainer inTag)
    {
        if (m_UnAcceptTaskMap.TryGetValue(inTag, out List<Task> list))
        {
            foreach (Task task in list)
            {
                if (task.TryTrigger(inTag))
                {
                    m_CurrentTaskMap.Add(task.Id, task);
                }
            }
        }
    }

    public void RemoveTask(Task task)
    {
        m_CurrentTaskMap.Remove(task.Id);
    }
}