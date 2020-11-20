using GameFramework;
using GameFramework.ObjectPool;
using MyAll;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UniversalEvent
{
    public class UniversalEventCore : SingletonModule<UniversalEventCore>, IGameModule
    {
        public DoublyLinkedList<UEvent> uNewEvents;
        public UEvent FireEvent(UEventSource source, string templateName) {
            if (source == null) {
                return null;
            }
            int hashCode = templateName.GetHashCode();
            return FireEvent(source, templateName);
        }

        public UEvent FireEvent(UEventSource source, int templateID) {
            if (source == null) {
                return null;
            }
            return;
        }


        private void RunEvent(UEventSource source, UEvent uEvent, bool newuEvent) {
            uEvent.eventOwner = source;
            uEvent.Start();
            if (newuEvent) {
                var track = uEvent.runningTrack;
                source.runningEvents[UEvent.GetRawRunningTrack(track)].AddToTail(uEvent);
                uNewEvents.AddToTail(uEvent);
            }
        }





        public string Name => throw new System.NotImplementedException();

        public void DestroyInstance()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }

        public void Init()
        {
            throw new System.NotImplementedException();
        }

        public void Open()
        {
            throw new System.NotImplementedException();
        }

        public void Reset(bool isShutDown = false)
        {
            throw new System.NotImplementedException();
        }

        public void Shutdown()
        {
            throw new System.NotImplementedException();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            throw new System.NotImplementedException();
        }
    }
}