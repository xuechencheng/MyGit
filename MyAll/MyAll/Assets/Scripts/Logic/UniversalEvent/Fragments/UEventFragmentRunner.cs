using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UniversalEvent
{
    public abstract class UEventFragmentRunner 
    {
        public UEvent fragmentOwner { get; set; }
        public virtual void Setup(UEventFragmentM data)
        {
        }
        public abstract void Execute();
    }
}