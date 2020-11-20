using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAll
{
    public class PlayerProperty : BaseProperty
    {
        protected bool m_bRunFast;//快跑模式
        public bool RunFast {
            get { return m_bRunFast; }
            set { m_bRunFast = value; }
        }
    }
}