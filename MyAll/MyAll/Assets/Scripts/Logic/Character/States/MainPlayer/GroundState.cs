using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyAll
{
    public class GroundState : MoveState
    {
        private bool m_bRunFast;
        public override void Execute(CharAI ai)
        {
            UpdateRunFast();
            UpdateDirAndSpeed();
            UpdateMove();
            UpdateAnimation();
        }

        private void UpdateRunFast() {
            MainPlayerProperty prop = m_AI.getOwner().BaseProp as MainPlayerProperty;
            if (!m_bRunFast)
            {
                if (prop.RunFast && prop.Speed > 0.1f)
                {
                    m_bRunFast = true;
                }
            }
            else {
                if (!prop.RunFast || prop.Speed < 0.1f) {
                    m_bRunFast = false;
                }
            }
        }

        public override void UpdateDirAndSpeed() {
            base.UpdateDirAndSpeed();
        }

        private void UpdateAnimation()
        {
            MainPlayerProperty prop = m_AI.getOwner().BaseProp as MainPlayerProperty;
            if (prop.Speed > 0) {
                if (m_bRunFast)
                {
                }
                else { 
                }
            }
        }
    }
}