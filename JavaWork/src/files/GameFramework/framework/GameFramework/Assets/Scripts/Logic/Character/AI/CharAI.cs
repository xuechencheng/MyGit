using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalEvent;

public class CharAI
{
    public StateMachine stateMachine;
    protected CharBase owner;
    private UEvent currentEvent;
    public UEvent FireUEvent(string templateName) {
        currentEvent = owner.uEventCore.FireEvent(owner.uEventSource, templateName);
        return currentEvent;
    }
}
