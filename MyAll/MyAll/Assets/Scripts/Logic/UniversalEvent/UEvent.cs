using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalEvent
{
    public unsafe class UEventSource { 
        
    }
    public unsafe class UEvent
    {
        UEventTemplateM* m_ptr;
        public UEventTemplateType templateType { get { return m_ptr->templateType; } }
        public int templateID { get { return m_ptr->templateID; } }

        public UEventSource eventOwner;
    }
}