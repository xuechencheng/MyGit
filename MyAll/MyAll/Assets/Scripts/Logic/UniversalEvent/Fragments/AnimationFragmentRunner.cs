using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UniversalEvent
{
    public unsafe class AnimationFragmentRunner : UEventFragmentRunner
    {
        AnimationFragmentM ptr;
        public override unsafe void Setup(void* data)
        {
            base.Setup(data);
            ptr = *(AnimationFragmentM*)data;
        }

        public override void Execute()
        {
            base.Execute();
            string stateName = ptr.stateName(fragmentOwner.templateID);
            if (fragmentOwner.eventOwner) { 
            }
        }
    }
}