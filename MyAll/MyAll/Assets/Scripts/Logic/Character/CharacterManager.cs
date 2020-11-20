using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAll
{
    public class CharacterManager : SingletonModule<CharacterManager>, IGameModule
    {
        public string Name => throw new System.NotImplementedException();

        private CharPlayerMain m_CharPlayerMain;

        public CharPlayerMain GetCharPlayerMain() {
            return m_CharPlayerMain;
        }



        public void DestroyInstance()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }

        public void Init()
        {
            throw new System.NotImplementedException();
        }

        public void Open()
        {
            throw new System.NotImplementedException();
        }

        public void Reset(bool isShutDown = false)
        {
            throw new System.NotImplementedException();
        }

        public void Shutdown()
        {
            throw new System.NotImplementedException();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            throw new System.NotImplementedException();
        }
    }
}