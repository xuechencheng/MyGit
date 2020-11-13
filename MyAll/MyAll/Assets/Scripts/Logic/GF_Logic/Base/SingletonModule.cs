using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyAll
{
    public abstract class SingletonModule<T> where T : class, IGameModule, new()
    {
        static protected T s_ModuleInstance;
        public static T ModuleInstance {
            get {
                if (s_ModuleInstance == null) {
                    s_ModuleInstance = ModuleManager.GetModule<T>();
                }
                return s_ModuleInstance;
            }
        }

        public static void DestoryInstanceSingleton() {
            s_ModuleInstance = null;
        }
    }
}