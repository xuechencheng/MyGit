using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game
{
    public abstract class SingletonModule<T> where T : class, IGameModule, new()
    {
        static protected T moduleInstance;
        public static T ModuleInstance {
            get {
                if (moduleInstance == null) {
                    moduleInstance = ModuleManager.GetModule<T>();
                }
                return moduleInstance;
            }
        }

        public static void DestroyInstanceSingleton() {
            moduleInstance = null;
        }
    }
}
