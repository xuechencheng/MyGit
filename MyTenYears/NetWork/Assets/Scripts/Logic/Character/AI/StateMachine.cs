using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Finish
/// </summary>
public class StateMachine
{
    public CharAI m_kOwner;
    protected Dictionary<int, State> m_States;
    protected State m_kCurrentState;
    protected State m_kNextState;

    public virtual void Init(CharAI ai) {
        m_kOwner = ai;
        m_States = new Dictionary<int, State>();
        RegisterStates();
    }

    public State GetState(CHARACTER_STATE eState) {
        State s = null;
        m_States.TryGetValue((int)eState, out s);
        return s;
    }

    public virtual void Update()
    {
        if (m_kCurrentState != null) {
            m_kCurrentState.Execute(m_kOwner);
        }
    }

    private void ChangeState(State newState) {
        if (m_kCurrentState != newState) {
            m_kNextState = newState;
            if (m_kCurrentState != null) {
                m_kCurrentState.Exit(m_kOwner);
            }
            m_kCurrentState = newState;
            m_kCurrentState.Enter(m_kOwner);
            m_kNextState = null;
        }
    }

    public void ChangeState(CHARACTER_STATE eState) {
        State s = GetState(eState);
        if (s!= null) {
            ChangeState(s);
        }
    }

    public State GetCurrentState() {
        return m_kCurrentState;
    }

    public CHARACTER_STATE GetCurrentStateType() {
        if (m_kCurrentState != null) {
            return m_kCurrentState.StateType;
        }
        return CHARACTER_STATE.CS_IDLE;
    }

    public virtual void OnLateUpdate() {
        if (m_kCurrentState != null) {
            m_kCurrentState.OnLateExecute(m_kOwner);
        }
    }

    public virtual void RegisterStates() { 
    }

    protected void DoRegisterState(State s, CHARACTER_STATE t) {
        s.SetStateMachine(this);
        s.StateType = t;
        m_States.Add((int)t, s);
    }

    public virtual void Exit() {
        if (m_kCurrentState != null) {
            m_kCurrentState.Exit(m_kOwner);
            m_kCurrentState = null;
        }
    }

    public virtual void Dispose() {
        Exit();
        if (m_States != null) {
            foreach (var state in m_States) {
                state.Value.Dispose();
            }
            m_States.Clear();
            m_States = null;
        }
        m_kOwner = null;
    }
}
