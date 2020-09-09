using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ObjectManager:Singleton<ObjectManager>
{
    //所有类资源池
    protected Dictionary<Type, object> m_ClassPoolDict = new Dictionary<Type, object>();
    /// <summary>
    /// 获取或创建类资源池
    /// </summary>
    public ClassObjectPool<T> GetOrCreateClassPool<T>(int maxCount) where T : class, new()
    {
        Type type = typeof(T);
        object obj = null;
        if (!m_ClassPoolDict.TryGetValue(type, out obj) || obj == null)
        {
            ClassObjectPool<T> newPool = new ClassObjectPool<T>(maxCount);
            m_ClassPoolDict.Add(type, newPool);
            return newPool;
        }
        return obj as ClassObjectPool<T>;
    }

}