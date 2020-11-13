using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameFramework
{
    public interface IGameModule {
        void Init();
        void Open();
        void Exit();
        void Update(float elapseSeconds, float realElapseSeconds);
        void Shutdown();

        void Reset(bool isShutDown = false);

        void DestroyInstance();
        string Name { get; }
    }
}