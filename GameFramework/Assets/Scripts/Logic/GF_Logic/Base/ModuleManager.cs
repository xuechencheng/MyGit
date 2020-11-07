using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game
{
    public static class ModuleManager
    {
        private static readonly List<IGameModule> gameFrameWorkModules = new List<IGameModule>(32);
        public static void Update(float elapseSeconds, float realElapseSeconds) {
            foreach (var module in gameFrameWorkModules) {
                module.Update(elapseSeconds, realElapseSeconds);
            }
        }
        /// <summary>
        /// 遍历并调用Shutdown和DestroyInstance
        /// </summary>
        public static void ShutDown() { 
        }

        public static T GetModule<T>() where T : class {
            Type type = typeof(T);
            string moduleName = string.Format("{0},{1}", type.Namespace, type.Name);
            Type moduleType = Type.GetType(moduleName);
            return GetModule(moduleType) as T;
        }

        private static IGameModule GetModule(Type moduleType) {
            foreach (var module in gameFrameWorkModules) {
                if (module.GetType() == moduleType) {
                    return module;
                }
            }
            return CreateModule(moduleType);
        }

        private static IGameModule CreateModule(Type moduleType)
        {
            IGameModule module = (IGameModule)Activator.CreateInstance(moduleType);
            if (module == null) { 
            }
            gameFrameWorkModules.Add(module);
            return module;
        }
    }
}