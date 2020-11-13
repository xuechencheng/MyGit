using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAll
{

    public struct StateEvent {
        public CHARACTER_STATE state;
    }
    public class StateMachine
    {
        public CharAI m_kOwner;
        protected Dictionary<int, State> m_States;
        protected State m_kCurrentState;
        protected State m_kNextState;
        protected State m_kPrevState;
        protected bool m_isPause;
        public virtual void Init(CharAI ai) {
            m_isPause = false;
            m_kOwner = ai;
            m_States = new Dictionary<int, State>();
            RegisterStates();
        }

        public State GetState(CHARACTER_STATE eState) {
            State s = null;
            m_States.TryGetValue((int)eState, out s);
            return s;
        }

        public void ChangeState(State newState) {
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
            State s;
            if (m_States.TryGetValue((int)eState, out s)) {
                ChangeState(s);
            }
        }

        public void SetCurrentState(State st) {
            m_kCurrentState = st;
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

        public void SetCurrentState(CHARACTER_STATE eState) {
            State s;
            if (m_States.TryGetValue((int)eState, out s)) {
                SetCurrentState(s);
            }
        }

        public void SetNextState(State st) {
            m_kNextState = st;
        }

        public void SetNextState(CHARACTER_STATE eState)
        {
            State s;
            if (m_States.TryGetValue((int)eState, out s))
            {
                SetNextState(s);
            }
        }

        public State GetNextState() {
            return m_kNextState;
        }
        public CHARACTER_STATE GetNextStateType()
        {
            if (m_kNextState != null)
            {
                return m_kNextState.StateType;
            }
            return CHARACTER_STATE.CS_IDLE;
        }

        public State GetPrevState() {
            return m_kPrevState;
        }

        public virtual void Update() {
            if (m_isPause) return;
            if (m_kNextState != null) {
                if (m_kNextState != m_kCurrentState) {
                    if (m_kCurrentState != null) {
                        m_kCurrentState.Exit(m_kOwner);
                    }
                    m_kPrevState = m_kCurrentState;
                    m_kCurrentState = m_kNextState;
                    m_kCurrentState.Enter(m_kOwner);
                }
                m_kNextState = null;
            }
            if (m_kCurrentState != null) {
                m_kCurrentState.Execute(m_kOwner);
            }
        }
        public virtual void RegisterStates() { 
        
        }

        public virtual void OnLateUpdate() {
            if (m_kCurrentState != null) {
                m_kCurrentState.OnLateExecute(m_kOwner);
            }
        }

        public virtual void ProcessMessage(StateEvent stEvent) { 
        
        }

        protected void DoRegisterState(State s, CHARACTER_STATE t) {
            s.SetStateMechina(this);
            s.StateType = t;
            m_States.Add( (int)t, s);
        }

        public virtual void Pause() {
            m_isPause = true;
        }

        public virtual void Resume() {
            m_isPause = false;
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
            m_kNextState = null;
            m_kPrevState = null;
        }
    }
}