using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public StateMachine stateMachine;
    private CHARACTER_STATE stateType;

    public CHARACTER_STATE StateType {
        get {
            return stateType;
        }
        set {
            stateType = value;
        }
    }

    public virtual void Exit(CharAI ai) { 
    }

    public virtual void Enter(CharAI ai) {
        
    }

    public virtual void Execute(CharAI ai) { 
    }

    public virtual void OnLateExecute(CharAI ai)
    {
    }

    public virtual void Dispose() { 
    
    }
}
