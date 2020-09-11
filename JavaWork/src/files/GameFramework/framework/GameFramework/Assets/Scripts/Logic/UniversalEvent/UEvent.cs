using GameFramework.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UniversalEvent
{
    public class UEventSource {
        public UEventTemplateM lastTemplate;
        public DoublyLinkedList<UEvent>[] runningEvents = new DoublyLinkedList<UEvent>[] {
            new DoublyLinkedList<UEvent>(),
            new DoublyLinkedList<UEvent>()
        };

    }
    public class UEvent
    {
        UEventTemplateM templateM;
        public UEventSource eventOwner;
        public int templateID {
            get { return templateM.templateID; }
        }
        public UEventFragmentRunner[] eventFragments;
        private int templateID1;
        protected float elpasedTime;
        public float normalizedTime;
        private float speed;
        private float duration;
        private readonly bool loop;

        public UEvent(int templateID1)
        {
            this.templateID1 = templateID1;
        }

        public bool isEnd { get; private set; }
        public bool isStop { get; private set; }
        public bool isPause { get; private set; }

        public bool isValid { get { return !isEnd && !isStop; } }

        public RunningTrack runningTrack { get; internal set; }

        public static int GetRawRunningTrack(RunningTrack track) {
            switch (track) {
                case RunningTrack.SingleAndCoutinue:
                case RunningTrack.SingleAndOverlay:
                    return (int)RawRunningTrack.Single;
                case RunningTrack.AddAndOverlay:
                case RunningTrack.AddAndCoutinue:
                case RunningTrack.Add:
                    return (int)RawRunningTrack.Add;
            }
            return (int)RawRunningTrack.Single;
        }

        public void Stop()
        {
            isStop = true;
            foreach (var f in eventFragments) {
                f.OnEventStop();
            }
        }

        internal void Start()
        {
            Reset();
        }

        public void Update(float deltaTime) {
            if (!isValid || isPause) {
                return;
            }
            deltaTime *= speed;
            var single_normalizedTime = elpasedTime;
            if (duration > 0) {
                normalizedTime += deltaTime / duration;
                single_normalizedTime = normalizedTime - (int)normalizedTime;
            }
            foreach (var fragment in eventFragments) {
                if (fragment.fired) {
                    continue;
                }
                if (single_normalizedTime >= fragment.fireTime) {
                    fragment.fired = true;
                    fragment.Execute();
                }
            }
            if (duration >= 0 && elpasedTime >= duration) {
                if (!loop)
                {
                    isEnd = true;
                    Stop();
                }
                else {
                    Rewind();
                }
            }
        }

        private void Rewind()
        {
            elpasedTime = 0;
            foreach (var fragment in eventFragments) {
                if (!fragment.fireOnce) {
                    fragment.fired = false;
                }
            }
        }

        internal void Reset()
        {
            elpasedTime = 0;
            normalizedTime = 0;
            isEnd = false;
            isStop = false;
            isPause = false;
            foreach (var fragment in eventFragments) {
                fragment.Reset();
            }
        }
    }
}