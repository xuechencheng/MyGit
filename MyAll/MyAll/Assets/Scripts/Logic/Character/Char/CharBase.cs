using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAll
{
    public class CharBase
    {
        protected ulong m_nCharID = 0;
        public ulong CharID {
            get {
                return m_nCharID;
            }
            set {
                m_nCharID = value;
            }
        }

        protected BaseProperty m_BaseProp;
        public BaseProperty BaseProp {
            get {
                return m_BaseProp;
            }
        }
    }
}