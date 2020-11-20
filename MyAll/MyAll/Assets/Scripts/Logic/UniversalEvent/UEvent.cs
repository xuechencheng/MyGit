using GameFramework.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalEvent
{
    public class UEventSource {
        public DoublyLinkedList<UEvent>[] runningEvents = new DoublyLinkedList<UEvent>[] { 
            new DoublyLinkedList<UEvent>(),new DoublyLinkedList<UEvent>()
        };
    }
    public class UEvent
    {
        public UEventSource eventOwner;
        private UEventFragmentM ptr;

        public RunningTrack runningTrack { get { return ptr.runningTrack; } }

        public void Start() {
            Reset();
        }

        public void Reset() { 
        }

        public static int GetRawRunningTrack(RunningTrack track) {
            switch (track) {
                case RunningTrack.SingleAndContinue:
                case RunningTrack.SingleAndOverlay:
                    return (int)RawRunningTrack.Single;
                case RunningTrack.AddAndContinue:
                case RunningTrack.AddAndOverlay:
                case RunningTrack.Add:
                    return (int)RawRunningTrack.Add;
            }
            return (int)RawRunningTrack.Single;
        }
    }
}