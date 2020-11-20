using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAll
{
    public class MoveState : State
    {
        protected CharAI m_AI;
        public virtual void UpdateMove() { }

        public virtual void UpdateDirAndSpeed() { 
        
        }
    }
}