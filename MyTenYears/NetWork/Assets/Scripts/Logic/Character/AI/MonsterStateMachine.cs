using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateMachine : StateMachine
{
    public override void RegisterStates()
    {
        base.RegisterStates();
        DoRegisterState(new MonsterIdleState(), CHARACTER_STATE.CS_IDLE);
        ChangeState(CHARACTER_STATE.CS_IDLE);
    }
}
