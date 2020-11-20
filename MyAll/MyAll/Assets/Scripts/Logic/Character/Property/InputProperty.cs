using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyAll
{
    public class InputProperty
    {
        private float m_fAxisH = 0;
        public float AxisH {
            get { return m_fAxisH; }
            set { m_fAxisH = value; }
        }

        private float m_fAxisV = 0;
        public float AxisV
        {
            get { return m_fAxisV; }
            set { m_fAxisV = value; }
        }

        public void Update()
        {
            PrecessMoveTouchAxis();
        }

        private void PrecessMoveTouchAxis() {
            float fH = 0;
            float fV = 0;
            //1,获取Lua全局变量设置fH和fV
#if UNITY_EDITOR
            //2，获取键盘输入值设置fH和fV
#endif
            //3，缓存变量
            m_fAxisH = fH;
            m_fAxisV = fV;
            //4,设置baseProperty的Runfast
            CharacterManager.ModuleInstance.GetCharPlayerMain().RunFast(true);
        }

    }
}