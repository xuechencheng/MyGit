using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UniversalEvent
{
    public unsafe class UEventFragmentRunner
    {
        public bool fired { get; set; }
        public UEvent fragmentOwner { get; set; }
        UEventFragmentM* ptr;
        public bool fireOnce { get { return ptr->fireOnce; } }
        public float fireTime { get { return ptr->fireTime; } }
        public virtual void Setup(void *data) {
            ptr = (UEventFragmentM*)data;
        }
        public virtual void Execute() { 
        }
        public virtual void OnEventStop() { 
        }

        public virtual void Reset() {
            fired = false;
        }
    }
}