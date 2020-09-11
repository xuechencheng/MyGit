using Game;
using GameFramework;
using GameFramework.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UniversalEvent
{
    public class UniversalEventCore : SingletonModule<UniversalEventCore>, IGameModule
    {
        private ObjectPool<int, UEvent> uEventPool = new ObjectPool<int, UEvent>();
        public DoublyLinkedList<UEvent> uNewEvents = new DoublyLinkedList<UEvent>();

        public UEvent FireEvent(UEventSource source, string templateName) {
            if (source == null) {
                return null;
            }
            int hashCode = templateName.GetHashCode();
            return FireEvent(source, hashCode);
        }

        public UEvent FireEvent(UEventSource source, int templateID) {
            if (source == null) { return null; }
            UEventTemplateM template = null;
            if (source.lastTemplate != null && source.lastTemplate.templateID == templateID)
            {
                template = source.lastTemplate;
            }
            else {
                template = UEventTemplateDataManager.get_event_data(templateID);
                source.lastTemplate = template;
            }
            UEvent uEvent = null;
            bool newUEvent = false;
            if (TryGenericEvent(source, template, out uEvent, ref newUEvent)) {
                RunEvent(source, uEvent, newUEvent);
                return uEvent;
            }
            return uEvent;
        }

        private void RunEvent(UEventSource source, UEvent uEvent, bool newUEvent)
        {
            RunningTrack track = uEvent.runningTrack;
            uEvent.eventOwner = source;
            uEvent.Start();
            if (newUEvent) {
                source.runningEvents[UEvent.GetRawRunningTrack(track)].AddToTail(uEvent);
                uNewEvents.AddToTail(uEvent);
            }
        }

        /// <summary>
        /// 返回是否需要执行uEvent
        /// </summary>
        /// <param name="source"></param>
        /// <param name="template"></param>
        /// <param name="uEvent"></param>
        /// <param name="newUEvent"></param>
        /// <returns></returns>
        private bool TryGenericEvent(UEventSource source, UEventTemplateM template, out UEvent uEvent, ref bool newUEvent)
        {
            var track = template.runningTrack;
            DoublyLinkedListNode<UEvent> node = null;
            switch (track) {
                case RunningTrack.SingleAndCoutinue:
                    node = source.runningEvents[UEvent.GetRawRunningTrack(track)].head;
                    while (node != null) {
                        if (node.t.templateID == template.templateID && node.t.isValid)
                        {
                            uEvent = node.t;
                            return false;
                        }
                        else {
                            node.t.Stop();
                        }
                        node = node.next;
                    }
                    break;
                case RunningTrack.SingleAndOverlay:
                    node = source.runningEvents[UEvent.GetRawRunningTrack(track)].head;
                    while (node != null)
                    {
                        if (node.t.templateID == template.templateID && node.t.isValid)
                        {
                            uEvent = node.t;
                            uEvent.Stop();
                            return true;
                        }
                        else
                        {
                            node.t.Stop();
                        }
                        node = node.next;
                    }
                    break;
                case RunningTrack.AddAndCoutinue:
                    node = source.runningEvents[UEvent.GetRawRunningTrack(track)].head;
                    while (node != null)
                    {
                        if (node.t.templateID == template.templateID && node.t.isValid)
                        {
                            uEvent = node.t;
                            return false;
                        }
                        node = node.next;
                    }
                    break;
                case RunningTrack.AddAndOverlay:
                    node = source.runningEvents[UEvent.GetRawRunningTrack(track)].head;
                    while (node != null)
                    {
                        if (node.t.templateID == template.templateID && node.t.isValid)
                        {
                            uEvent = node.t;
                            uEvent.Stop();
                            return true;
                        }
                        node = node.next;
                    }
                    break;
                case RunningTrack.Add:
                    break;
            }
            uEvent = GenericEvent(template.templateID);
            newUEvent = true;
            return true;
        }

        private UEvent GetUEventFromPool(int templateID) {
            return uEventPool.Get(templateID);
        }

        private UEvent GenericEvent(int templateID)
        {
            UEvent uevent = GetUEventFromPool(templateID);
            if (uevent == null)
            {
                uevent = new UEvent(templateID);
            }
            else {
                uevent.Reset();
            }
            return uevent;
        }

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

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public void ShutDown()
        {
            throw new System.NotImplementedException();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            throw new System.NotImplementedException();
        }
    }
}