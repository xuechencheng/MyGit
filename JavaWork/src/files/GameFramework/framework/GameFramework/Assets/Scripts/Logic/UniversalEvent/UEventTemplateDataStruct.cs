using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UniversalEvent
{
    /// <summary>
    /// 原来是Struct
    /// </summary>
    public class UEventTemplateM {
        public int templateID;
        public byte runningTrack_enum_value;
        public RunningTrack runningTrack {
            get {
                return (RunningTrack)runningTrack_enum_value;
            }
        }
    }
    /// <summary>
    /// 原来是Struct
    /// </summary>
    public class UEventFragmentM
    {
        internal bool fireOnce;
    }
}