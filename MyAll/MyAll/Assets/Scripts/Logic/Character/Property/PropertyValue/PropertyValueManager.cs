using GameFramework;
using MyAll;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyValueManager : SingletonModule<PropertyValueManager>, IGameModule
{
    public string Name => "PropertyValueManager";
    const int MaxEventLimitInOneFrame = 50;
    int m_LastEventCountInOneFrame = 0;
    Queue<IPropertyValue> m_FiredSetEventQueue;

    public void Init()
    {
        m_FiredSetEventQueue = new Queue<IPropertyValue>();
    }

    public void Open()
    {
    }

    public void Exit()
    {
    }

    public void Update(float elapseSeconds, float realElapseSeconds)
    {
        m_LastEventCountInOneFrame = MaxEventLimitInOneFrame;
        while (m_FiredSetEventQueue.Count > 0 && m_LastEventCountInOneFrame > 0) {
            m_LastEventCountInOneFrame--;
            var del = m_FiredSetEventQueue.Dequeue();
            if (del != null) {
                del.HandleChanged();
            }
            del = null;
        }
    }

    public void PushSetEvent(IPropertyValue property) {
        m_FiredSetEventQueue.Enqueue(property);
    }

    public void Shutdown()
    {
        Reset(true);
    }

    public void Reset(bool isShutDown = false)
    {
        m_FiredSetEventQueue.Clear();
    }

    public void DestroyInstance()
    {
        DestoryInstanceSingleton();
    }
}
