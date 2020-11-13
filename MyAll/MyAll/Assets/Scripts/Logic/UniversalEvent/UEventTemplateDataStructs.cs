using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UniversalEvent
{
    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public struct UEventTemplateM {
        [FieldOffset(0)]
        public byte typeKey;
        [FieldOffset(1)]
        public byte templateType_enum_value;
        [FieldOffset(2)]
        public int templateID;
        [FieldOffset(6)]
        public int name_str_addr;
        [FieldOffset(10)]
        public byte group;
        [FieldOffset(11)]
        public float duration;
        [FieldOffset(15)]
        public bool loop;
        [FieldOffset(16)]
        public bool global;
        [FieldOffset(17)]
        public byte runningTrack_enum_value;
        [FieldOffset(18)]
        public int eventFragmentTemplates_add_addr;
        [FieldOffset(22)]
        public byte __padding0;
        [FieldOffset(23)]
        public byte __padding1;

        public UEventTemplateType templateType {
            get {
                return (UEventTemplateType)templateType_enum_value;
            }
        }

        public string name {
            get {
                var eventT = UEventTemplateDataManager.get_event_type(templateID);
                return UEventTemplateDataManager.read_string(name_str_addr, eventT);
            }
        }

        public RunningTrack runningTrack {
            get {
                return (RunningTrack)runningTrack_enum_value;
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct UEventFragmentM {
        [FieldOffset(0)]
        public byte typeKey;
        [FieldOffset(1)]
        public float fireTime;
        [FieldOffset(5)]
        public bool fireOnce;
        [FieldOffset(6)]
        public byte __padding0;
        [FieldOffset(7)]
        public byte __padding1;
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct AnimationFragmentM
    {
        [FieldOffset(0)]
        public byte typeKey;
        [FieldOffset(1)]
        public float fireTime;
        [FieldOffset(5)]
        public bool fireOnce;
        [FieldOffset(6)]
        public byte __padding0;
        [FieldOffset(7)]
        public byte __padding1;
        [FieldOffset(8)]
        public byte stateName_str_addr;
        [FieldOffset(12)]
        public float __padding2;
        [FieldOffset(13)]
        public bool __padding3;
        [FieldOffset(14)]
        public byte __padding4;
        [FieldOffset(15)]
        public byte __padding5;

        public string stateName(int templateID) {
            var eventT = UEventTemplateDataManager.get_event_type(templateID);
            return UEventTemplateDataManager.read_string(stateName_str_addr, eventT);
        }
    }
}