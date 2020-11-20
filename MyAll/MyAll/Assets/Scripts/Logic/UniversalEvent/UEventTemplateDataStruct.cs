using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalEvent
{
    public class UEventFragmentM { 
        public RunningTrack runningTrack { get { return RunningTrack.Add; } }
    }

    public class AnimationFragmentM : UEventFragmentM{
        public string StateName(int templateID) {
            return "";
        }
    }
}