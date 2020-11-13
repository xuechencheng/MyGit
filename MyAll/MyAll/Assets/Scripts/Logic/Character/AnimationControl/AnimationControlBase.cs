using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAll
{
    public class AnimationControlBase
    {
        protected CharBase m_kOwner;
        public float FrameSpeed = 1.0f;
        protected float m_speedScale = 1.0f;
        public float SpeedScale {
            get { return m_speedScale; }
        }
    }
}