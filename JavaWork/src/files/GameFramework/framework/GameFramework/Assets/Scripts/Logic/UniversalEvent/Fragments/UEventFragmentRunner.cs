using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalEvent;

public class UEventFragmentRunner
{
    private UEventFragmentM eventFragmentM;

    public bool fired { get; set; }
    public float fireTime { get; internal set; }
    public bool fireOnce { get { return eventFragmentM.fireOnce; }}

    public virtual void OnEventStop() { 
        
    }

    internal void Reset()
    {
        fired = false;
    }

    internal virtual void Execute()
    {
        throw new NotImplementedException();
    }
}
