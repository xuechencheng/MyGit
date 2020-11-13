using MyAll;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
namespace UniversalEvent
{
    public enum FragmentType { 
        UEventTemplateContainer = 0,
        UEventTemplate,
        UEventFragment,
        AudioFragment,
        UIAudioFragment,
        AniamtionFragment,
        ParticleFragment,
        UIParticleFragment,
        CameraEffectFragment,
        ScaleAnimationSpeedFragment,
        FocusCameraFragment,
        ChangeAttachPoint,
        ChangeSubAttachPoint,
        ChromaticAberrationFrag,
        CameraZRollEffectFrag,
        SetGamePlayCameraView,
        MaterialTypeFoot,
        PlayLayerAniamtion,
        PlaySceneVCamera,
        StopSceneVCamera,
        SetBindingObject,
        SkillCameraEffectFragment,
        SetAnimatorCullingModeFragment,
        CameraParticleFragment,
        CameraRedirectFragment,
        CameraDistanceFragment,
        CameraOffsetFragment,
    }
    public enum UEventTemplateType
    {
        Default,
        UI,
    }

    public enum RunningTrack { 
        SingleAndContinue,
        SingleAndOverlay,
        AddAndContinue,
        AddAndOverlay,
        Add,
    }

    public enum RawRunningTrack { 
        Single = 0,
        Add = 1,
    }

    public unsafe class FragEntry {
        public int typeKey;
        public int count;
        public int blob;
        public int struct_size;
        private byte* intptr;
        public void SetPtr(byte *ptr) {
            intptr = ptr;
        }

        public byte* GetPtr(int index) {
            return intptr + struct_size * index;
        }
    }

    public enum UEventType
    {
        Default = 0,
        UI = 1,
    }
    public unsafe class UEventTemplateData {
        public int event_count;
        public int fileLength;
        public int frag_type_count;
        public int event_blob;
        public int arr_Blob;
        public int str_Blob;
        public int m_Position = 0;
        public byte* m_pData;
        public UEventTemplateM* p_events;

        public Dictionary<int, FragEntry> fragType = new Dictionary<int, FragEntry>();
        public Dictionary<int, string> cacheString = new Dictionary<int, string>();
    }
    public struct UEventTypeIndex
    {
        public UEventType type;
        public int index;
    }

    public unsafe partial class UEventTemplateDataManager
    {
        public static Dictionary<int, Type> runnerMap = new Dictionary<int, Type>() {
            { (int)FragmentType.UEventFragment,typeof(UEventFragmentRunner)},
            { (int)FragmentType.AniamtionFragment,typeof(AnimationFragmentRunner)},
        };

        private static Dictionary<UEventType, UEventTemplateData> uEventTemplates = new Dictionary<UEventType, UEventTemplateData>();

        private static Dictionary<int, UEventTypeIndex> templateIdIndex;

        public static int EventCount {
            get {
                int event_count = 0;
                foreach (var kvp in uEventTemplates) {
                    event_count += kvp.Value.event_count;
                }
                return event_count;
            }
        }

        public static void Setup() {
            ClearSetUp();
            string path = AssetUtility.GetUEventTemplatesBinaryPath();
            byte[] bytes = File.ReadAllBytes(path);
            Setup(bytes, UEventType.Default);

        }

        public static void Setup(byte[] bytes, UEventType uEventType)
        {
            var uEventTemplate = new UEventTemplateData();
            if (!uEventTemplates.ContainsKey(uEventType))
            {
                uEventTemplates.Add(uEventType, uEventTemplate);
            }
            else {
                uEventTemplates[uEventType] = uEventTemplate;
            }
            uEventTemplate.fileLength = bytes.Length;
            uEventTemplate.m_Position = 0;
            IntPtr p = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, p, bytes.Length);
            uEventTemplate.m_pData = (byte*)p;
            uEventTemplate.event_count = read_int(uEventTemplate);
            uEventTemplate.event_blob = read_int(uEventTemplate);
            uEventTemplate.p_events = (UEventTemplateM*)(uEventTemplate.m_pData + uEventTemplate.event_blob);
            uEventTemplate.arr_Blob = read_int(uEventTemplate);
            uEventTemplate.str_Blob = read_int(uEventTemplate);
            uEventTemplate.frag_type_count = read_int(uEventTemplate);
            for (int i = 0; i < uEventTemplate.frag_type_count; i++) {
                FragEntry entry = new FragEntry();
                entry.typeKey = read_byte(uEventTemplate);
                entry.count = read_int(uEventTemplate);
                entry.blob = read_int(uEventTemplate);
                entry.SetPtr(uEventTemplate.m_pData + entry.blob);
                if (!uEventTemplate.fragType.ContainsKey(entry.typeKey)) {
                    uEventTemplate.fragType.Add(entry.typeKey,entry);
                }
            }
            for (int i = 0; i < uEventTemplate.event_count; i++) {
                byte typeKey = (uEventTemplate.p_events + i)->typeKey;
                byte templateType_enum_value = (uEventTemplate.p_events + i)->templateType_enum_value;
                int templateID = (uEventTemplate.p_events + i)->templateID;
                int name_str_addr = (uEventTemplate.p_events + i)->name_str_addr;
                byte group = (uEventTemplate.p_events + i)->group;
                float duration = (uEventTemplate.p_events + i)->duration;
                bool loop = (uEventTemplate.p_events + i)->loop;
                if (!templateIdIndex.ContainsKey(templateID)) {
                    UEventTypeIndex uEventTypeIndex = new UEventTypeIndex();
                    uEventTypeIndex.type = uEventType;
                    uEventTypeIndex.index = i;
                    templateIdIndex.Add(templateID, uEventTypeIndex);
                }
            }

        }

        public static void ClearSetUp() {
            foreach (var kvp in uEventTemplates) {
                Marshal.FreeHGlobal((IntPtr)kvp.Value.m_pData);
                kvp.Value.m_pData = null;
                kvp.Value.fragType.Clear();
                kvp.Value.cacheString.Clear();
            }
            uEventTemplates.Clear();
            templateIdIndex = new Dictionary<int, UEventTypeIndex>(5000);
        }

        private static byte read_byte(UEventTemplateData uEventTemplate) {
            byte* ptr = uEventTemplate.m_pData + uEventTemplate.m_Position;
            if (ptr == null) {
                return 0;
            }
            byte ret = *ptr;
            uEventTemplate.m_Position += 1;
            return ret;
        }

        private static short read_short(UEventTemplateData uEventTemplate)
        {
            byte* ptr = uEventTemplate.m_pData + uEventTemplate.m_Position;
            if (ptr == null)
            {
                return 0;
            }
            short ret = *(short*)ptr;
            uEventTemplate.m_Position += 2;
            return ret;
        }

        private static int read_int(UEventTemplateData uEventTemplate)
        {
            byte* ptr = uEventTemplate.m_pData + uEventTemplate.m_Position;
            if (ptr == null)
            {
                return 0;
            }
            int ret = *(int*)ptr;
            uEventTemplate.m_Position += 4;
            return ret;
        }

        public static UEventType get_event_type(int templateId)
        {
            UEventTypeIndex uEventTypeIndex;
            templateIdIndex.TryGetValue(templateId, out uEventTypeIndex);
            return uEventTypeIndex.type;
        }

        public static string read_string(int str_addr, UEventType uEventType) {
            string ret = string.Empty;
            UEventTemplateData uEventTemplate;
            if (uEventTemplates.TryGetValue(uEventType, out uEventTemplate)) {
                int oldPos = uEventTemplate.m_Position;
                if (str_addr >= 0) {
                    if (!uEventTemplate.cacheString.TryGetValue(str_addr, out ret)) {
                        uEventTemplate.m_Position = uEventTemplate.str_Blob + str_addr;
                        short len = read_short(uEventTemplate);
                        if (len > 0) {
                            sbyte* ptr = (sbyte*)(uEventTemplate.m_pData + uEventTemplate.m_Position);
                            if (ptr != null) {
                                ret = new string(ptr, 0, len, Encoding.UTF8);
                            }
                        }
                        uEventTemplate.cacheString.Add(str_addr, ret);
                    }
                }
                uEventTemplate.m_Position = oldPos;
            }
            return ret;
        }

        public static UEventFragmentRunner get_runner(int typekey, int index, UEventType uEventType) {
            UEventTemplateData uEventTemplate;
            if (uEventTemplates.TryGetValue(uEventType, out uEventTemplate)) {
                byte* intptr = (byte*)0;
                FragEntry entry = null;
                if (!uEventTemplate.fragType.TryGetValue(typekey, out entry)) {
                    return null;
                }
                int addr = entry.blob + (index + 1) * entry.struct_size;
                if (addr > uEventTemplate.fileLength) {
                    return null;
                }
                intptr = entry.GetPtr(index);
                if (intptr == null) {
                    return null;
                }
                Type type = null;
                if (!runnerMap.TryGetValue(typekey, out type)) {
                    return null;
                }
                UEventFragmentRunner ret = (UEventFragmentRunner)Activator.CreateInstance(type);
                ret.Setup(intptr);
                return ret;
            }
            return null;
        }

        public static void Destory() {
            ClearSetUp();
        }
    }
}