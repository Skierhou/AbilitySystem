using System;
using System.Collections.Generic;

public class ClassObjectPool<T> where T : class, new()
{
    //池子
    protected Stack<T> m_Pool = new Stack<T>();
    //最大个数
    protected int m_MaxCount = 0;
    //正在使用的个数（没被回收的个数）
    protected int m_UseCount = 0;

    public ClassObjectPool(int maxCount)
    {
        m_MaxCount = maxCount;
        for (int i = 0; i < m_MaxCount; i++)
        {
            m_Pool.Push(new T());
        }
    }
    public T Spawn(bool isCreateNewClass = true)
    {
        if (m_Pool.Count > 0)
        {
            T res = m_Pool.Pop();
            if (res == null)
            {
                if (isCreateNewClass)
                {
                    res = new T();
                }
            }
            m_UseCount++;

            return res;
        }
        else
        {
            if (isCreateNewClass)
            {
                T res = new T();
                m_UseCount++;
                return res;
            }
        }
        return null;
    }
    public bool UnSpawn(T obj)
    {
        if (obj == null) return false;

        m_UseCount--;
        if (m_Pool.Count >= m_MaxCount && m_MaxCount > 0)
        {
            obj = null;
            return false;
        }
        else
        {
            m_Pool.Push(obj);
            return true;
        }
    }
}