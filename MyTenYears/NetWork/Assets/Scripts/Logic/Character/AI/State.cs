using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected StateMachine m_kMachine;
    protected CHARACTER_STATE m_eStateType;
    public CHARACTER_STATE StateType {
        get { return m_eStateType; }
        set { m_eStateType = value; }
    }

    public void SetStateMachine(StateMachine machine) {
        m_kMachine = machine;
    }

    public virtual void Enter(CharAI ai){

    }

    public virtual void Execute(CharAI ai)
    {

    }

    public virtual void OnLateExecute(CharAI ai)
    {

    }

    public virtual void Exit(CharAI ai) { 
    
    }

    public virtual void Dispose() {
        m_kMachine = null;
    }

}
