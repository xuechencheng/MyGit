using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAll
{
    public class State
    {
        protected CHARACTER_STATE m_eStateType;
        public CHARACTER_STATE StateType {
            get { return m_eStateType; }
            set { m_eStateType = value; }
        }
        protected StateMachine m_kMachine;
        protected bool m_isPause = false;
        public void SetStateMechina(StateMachine machine) {
            m_kMachine = machine;
        }

        public virtual void Enter(CharAI ai) { 
        }

        public virtual void Execute(CharAI ai)
        {
        }
        public virtual void OnLateExecute(CharAI ai)
        {
        }
        public virtual void Exit(CharAI ai)
        {
        }

        public virtual void SetParam(object param) { 
        }
        public virtual void SetParam(Vector3 param)
        {
        }
        public virtual void SetParam(int param)
        {
        }
        public virtual void SetParam(float param1, float param2)
        {
        }
        public virtual void SetParam(ArrayList param)
        {
        }
        public virtual bool IsCanGotoNewState(CHARACTER_STATE NewStateType) {
            return true;
        }
        public virtual void Pause() {
            m_isPause = true;
        }

        public virtual void Resume() {
            m_isPause = false;
        }

        public virtual void Dispose() {
            m_kMachine = null;
        }
    }
}