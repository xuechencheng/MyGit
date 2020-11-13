using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyValue<T> : IPropertyValue
{
    protected T m_OldValue;
    protected T m_Value;
    protected bool changed;

    private T m_oldValueForSend;
    private T m_valueForSend;

    public virtual T Value {
        get {
            return m_Value;
        }
        set {
            m_OldValue = m_Value;
            m_Value = value;
            if ((m_Value == null && m_OldValue != null) || (m_Value != null && !m_Value.Equals(m_OldValue))) {
                if (m_OnValueChanged != null) {
                    changed = true;
                    m_oldValueForSend = m_OldValue;
                    m_valueForSend = m_Value;
                    PropertyValueManager.ModuleInstance.PushSetEvent(this);
                }
            }
        }
    }

    public PropertyValue() {
        m_Value = default(T);
    }

    protected Action<T, T> m_OnValueChanged;
    public void AddListener(Action<T,T> callback) {
        m_OnValueChanged -= callback;
        m_OnValueChanged += callback;
    }

    public void RemoveListner(Action<T, T> callback) {
        m_OnValueChanged -= callback;
    }

    public virtual void ForceUpdate() {
        changed = true;
        m_oldValueForSend = m_valueForSend = m_Value;
        HandleChanged();
    }

    public void HandleChanged()
    {
        if (m_OnValueChanged != null) {
            if (changed) {
                changed = false;
                m_OnValueChanged.Invoke(m_oldValueForSend, m_valueForSend);
            }
        }
    }

    public void Reset() {
        m_Value = default(T);
        Value = default(T);
    }
}
