using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public override void RegisterStates()
    {
        base.RegisterStates();
        DoRegisterState(new PlayerMoveState(), CHARACTER_STATE.CS_MOVE);
        ChangeState(CHARACTER_STATE.CS_MOVE);
    }
}
