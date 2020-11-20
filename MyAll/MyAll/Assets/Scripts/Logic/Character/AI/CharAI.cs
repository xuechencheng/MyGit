using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalEvent;

namespace MyAll
{
    public class CharAI
    {
        private CharBase m_kOwner;
        public CharBase getOwner() {
            return m_kOwner;
        }
        public UEvent FireUEvent(string templateName) {
            return null;
        }
    }
}