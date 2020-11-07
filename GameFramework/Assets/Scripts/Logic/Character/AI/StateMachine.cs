using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct StateEvent {
    public CHARACTER_STATE state;
}
public class StateMachine
{
    public CharAI owner;
    protected Dictionary<int, State> states;
    protected State currentState;
    protected State nextState;
    protected State prevState;
    protected bool isPause;
    /// <summary>
    /// 1，初始化变量 2，注册状态机
    /// </summary>
    /// <param name="charAI"></param>
    public virtual void Init(CharAI charAI) { 
        
    }
    /// <summary>
    /// 更改数据，调用State的Exit和Enter
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(CHARACTER_STATE newState) {
        
    }
    /// <summary>
    /// 1，判断是否暂停 2，如果需要切换到NextState 3，调用当前State的Excute方法
    /// </summary>
    public virtual void Update() { 
        
    }
    /// <summary>
    /// 注册状态机，空
    /// </summary>
    public virtual void RegisterStates() { 
    
    }
    /// <summary>
    /// 调用CurrentState的OnLateExecute
    /// </summary>
    public virtual void OnLateUpdate() { 
    
    }

    /// <summary>
    /// 根据不同情况，处理切换状态消息，空
    /// </summary>
    /// <param name="stateEvent"></param>
    public virtual void ProcessMessage(StateEvent stateEvent) { 
    
    }
    /// <summary>
    /// 加到states中
    /// </summary>
    /// <param name="s"></param>
    /// <param name="state"></param>
    protected void DoRegisterState(State s, CHARACTER_STATE state) { 
    }
    /// <summary>
    /// 1，调用每个State的Dispose 2，清理变量
    /// </summary>
    public virtual void Dispose() { 
    
    }
}
