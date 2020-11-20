using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalEvent {
    public enum RunningTrack { 
        SingleAndContinue,
        SingleAndOverlay,
        AddAndContinue,
        AddAndOverlay,
        Add,
    }

    public enum RawRunningTrack { 
        Single = 0,
        Add = 1,
    }
}