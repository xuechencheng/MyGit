using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace MyAll
{
    public static class ModuleManager
    {
        private static readonly List<IGameModule> s_GameFrameworkModules = new List<IGameModule>(32);
        private static Dictionary<Type, Type> interfaceMapping = new Dictionary<Type, Type>();

        public static void Update(float elapseSeconds, float realElapseSeconds) {
            if (s_GameFrameworkModules.Count > 0) {
                foreach (IGameModule item in s_GameFrameworkModules) {
                    Profiler.BeginSample(item.Name);
                    item.Update(elapseSeconds, realElapseSeconds);
                    Profiler.EndSample();
                }
            }
        }

        public static T GetModule<T>() where T : class {
            Type type = typeof(T);
            string moduleName;
            if (type.IsInterface) {
                Type rawType = null;
                if (!interfaceMapping.TryGetValue(type, out rawType)) { 
                    moduleName = string.Format("{0}.{1}" ,type.Namespace, type.Name.Substring(1));
                    rawType = Type.GetType(moduleName);
                    if (type == null) {
                        
                    }
                    interfaceMapping.Add(type, rawType);
                }
                type = rawType;
            }
            return GetModule(type) as T;
        }

        private static IGameModule GetModule(Type moduleType)
        {
            foreach (IGameModule item in s_GameFrameworkModules) {
                if (item.GetType() == moduleType) {
                    return item;
                }
            }
            IGameModule module = (IGameModule)Activator.CreateInstance(moduleType);
            if (module == null) {
                return null;
            }
            s_GameFrameworkModules.Add(module);
            return module;
        }

        public static void Shutdown() {
            foreach (IGameModule item in s_GameFrameworkModules)
            {
                item.Shutdown();
                item.DestroyInstance();
            }
            s_GameFrameworkModules.Clear();
        }

        public static void ResetAll() {
            foreach (IGameModule item in s_GameFrameworkModules)
            {
                item.Reset();
            }
        }
    }
}