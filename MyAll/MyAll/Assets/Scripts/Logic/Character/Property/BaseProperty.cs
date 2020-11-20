using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAll
{
    public class BaseProperty
    {
        public PropertyValue<string> Name = new PropertyValue<string>();
        protected float m_fSpeed;
        public float Speed {
            get {
                return m_fSpeed;
            }
            set {
                m_fSpeed = value;
            }
        }
    }
}