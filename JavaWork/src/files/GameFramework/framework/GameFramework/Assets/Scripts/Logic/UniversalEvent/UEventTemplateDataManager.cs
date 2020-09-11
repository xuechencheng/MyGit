using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalEvent;

public class UEventTemplateDataManager
{
    public static UEventTemplateM get_event_data(int templateId) {
        return null;
    }
}

public enum RunningTrack { 
    SingleAndCoutinue,
    SingleAndOverlay,
    AddAndCoutinue,
    AddAndOverlay,
    Add,
}
public enum RawRunningTrack { 
    Single = 0,
    Add = 1,
}