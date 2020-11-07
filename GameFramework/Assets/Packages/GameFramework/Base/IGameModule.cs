using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFramework { 
    public interface IGameModule
    {
        void Init();
        void Open();
        void Exit();
        void Update(float elapseSeconds, float realElapseSeconds);
        void ShutDown();
        void Reset();
        void DestroyInstance();
    }
}
